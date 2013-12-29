using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using SF.Reflection;

namespace SF.Expressions
{
    public static partial class ExpressionEx
    {
        /// <summary>
        ///     Normalizes ET1 to ET2
        /// </summary>
        /// <param name="expression">Expression to normalize</param>
        /// <returns>Normalized expression</returns>
        public static Expression Normalize(Expression expression)
        {
            return NormalizationVisitor.Instance.Visit(expression);
        }

        /// <summary>
        ///     Normalizes ET1 to ET2
        /// </summary>
        /// <param name="expression">Expression to normalize</param>
        /// <returns>Normalized expression</returns>
        public static T Normalize<T>(T expression)
            where T : Expression
        {
            return (T) Normalize((Expression) expression);
        }

        /// <summary>
        ///     Checks if expression is expression can be assigned to
        /// </summary>
        public static bool IsLValue(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((MemberExpression) expression).Member.CanWrite();
                case ExpressionType.Parameter:
                    return true;
                case ExpressionType.Index:
                    var indexExpression = (IndexExpression) expression;
                    return indexExpression.Indexer == null || indexExpression.Indexer.CanWrite;
            }
            return false;
        }

        /// <summary>
        ///     Returns type for ParameterExpression including ByRef modifier
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ParameterType(this ParameterExpression expression)
        {
            return !expression.IsByRef ? expression.Type : expression.Type.MakeByRefType();
        }

        /// <summary>
        ///     Check if lambda's signature matches parameters and result
        /// </summary>
        public static bool SignatureMatch(LambdaExpression lambda, Type result, params Type[] parameters)
        {
            return lambda.Parameters.Count == parameters.Length
                   && lambda.ReturnType == result
                   && !parameters.Where((t, i) => lambda.Parameters[i].ParameterType() != t).Any();
        }

        /// <summary>
        ///     Replaces parameter in expression using replace function
        /// </summary>
        public static Expression ReplaceParameters(Expression expression, Func<ParameterExpression, Expression> replace)
        {
            var visitor = new ReplaceParametersVisitor(replace);
            return visitor.Visit(expression);
        }

        /// <summary>
        ///     Replaces parameter in expression to new parameter
        /// </summary>
        public static Expression ReplaceParameter(Expression expression, ParameterExpression parameter, Expression replace)
        {
            return replace == parameter ? expression : ReplaceParameters(expression, p => p == parameter ? replace : p);
        }

        /// <summary>
        ///     Returns lambda expression body with parameters repalced with passed arguments
        /// </summary>
        public static Expression Inline(LambdaExpression expression, params Expression[] arguments)
        {
            if (arguments.Length != expression.Parameters.Count)
                throw new ArgumentException("Parameter count does not match");
            if (arguments.Length == 1)
                return ReplaceParameter(expression.Body, expression.Parameters[0], arguments[0]);
            var lambdaParams = expression.Parameters;
            return ReplaceParameters(expression.Body, p => arguments[lambdaParams.IndexOf(p)]);
        }

        /// <summary>
        ///     Returns delegate type with parameters and result
        /// </summary>
        public static Type GetDelegateType(Type result, params Type[] parameters)
        {
            var types = new Type[parameters.Length + 1];
            Array.Copy(parameters, types, parameters.Length);
            types[parameters.Length] = result;
            return Expression.GetDelegateType(types);
        }

        /// <summary>
        ///     Converts lambda input and return types
        /// </summary>
        /// <param name="lambda">lambda to convert</param>
        /// <param name="delegateType">new delegate type</param>
        /// <returns></returns>
        public static LambdaExpression ConvertLambda(LambdaExpression lambda, Type delegateType)
        {
            if (lambda.Type == delegateType)
                return lambda;

            var invoke = SimpleDelegate.GetInvokeMethod(delegateType);

            var result = invoke.ReturnType;
            var invokeParameters = invoke.GetParameters();
            var parameters = Array.ConvertAll(invokeParameters, p => p.ParameterType);

            if (parameters.Length < lambda.Parameters.Count)
                throw new ArgumentException("Parameters count is less than lambdas", "delegateType");

            if (SignatureMatch(lambda, result, parameters))
            {
                // Fixes stupid issue in .Net framework - byref delegates are not convertible, so can't compile byref =\
                return delegateType == null ? lambda : Expression.Lambda(delegateType, lambda.Body, lambda.Parameters);
            }

            var newParameters = Array.ConvertAll(parameters, Expression.Parameter);

            var variables = new List<ParameterExpression>();
            var inConversions = new List<Expression>();
            var outConversions = new List<Expression>();
            var call = new Expression[lambda.Parameters.Count];
            for (var i = 0; i < lambda.Parameters.Count; i++)
            {
                var lambdaParam = lambda.Parameters[i];
                var newParam = newParameters[i];
                if (lambdaParam.Type == newParam.Type)
                {
                    // no conversion
                    call[i] = newParam;
                    continue;
                }
                if (!(newParam.IsByRef && lambdaParam.IsByRef))
                {
                    //call[i] = newParam.TypeAs(lambdaParam.Type);
                    call[i] = newParam.Convert(lambdaParam.Type);
                    continue;
                }
                var variable = Expression.Variable(lambdaParam.Type);
                call[i] = variable;
                variables.Add(variable);
                inConversions.Add(invokeParameters[i].IsOut
                    ? variable.Assign(Expression.Default(lambdaParam.Type))
                    : variable.Assign(newParam.Convert(lambdaParam.Type)));
                outConversions.Add(newParam.Assign(variable.Convert(newParam.Type)));
            }
            var newBody = Inline(lambda, call);

            if (variables.Count == 0)
            {
                if (result == newBody.Type) 
                    return Expression.Lambda(delegateType, newBody, newParameters);
                if (result == typeof (void) || newBody.Type == typeof (void))
                    newBody = Expression.Block(newBody, Expression.Default(result));
                else
                    newBody = newBody.Convert(result);
            }
            else
            {
                if (result == typeof (void) || newBody.Type == typeof (void))
                    outConversions.Add(Expression.Default(result));
                else
                {
                    var resultVariable = Expression.Variable(result);
                    newBody = Expression.Assign(resultVariable, newBody.Convert(result));
                    variables.Add(resultVariable);
                    outConversions.Add(resultVariable);
                }
                inConversions.Add(newBody);
                newBody = Expression.Block(variables, inConversions.Concat(outConversions));
            }

            return Expression.Lambda(delegateType, newBody, newParameters);
        }

        /// <summary>
        ///     Compiles lambda ad TDelegate
        /// </summary>
        public static TDelegate CompileAs<TDelegate>(this LambdaExpression lambda)
            where TDelegate : class
        {
            var converted = ConvertLambda(lambda, typeof (TDelegate));
            return converted.Compile() as TDelegate;
        }

        /// <summary>
        ///     Returns debug string with expressions body
        /// </summary>
        public static string ToDebugString(this Expression expression)
        {
            var visitor = new DebugStringVisitor();
            visitor.Visit(expression);
            return visitor.ToString();
        }

        /// <summary>
        ///     Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="canBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression, Func<Expression, bool> canBeEvaluated)
        {
            return new PartialEvaluator(new PartialEvaluationSearcher(canBeEvaluated).Process(expression)).Eval(expression);
        }

        /// <summary>
        ///     Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression)
        {
            return PartialEval(expression, CanBeEvaluatedLocally);
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }
    }
}