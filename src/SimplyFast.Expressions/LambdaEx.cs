using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SF.Reflection;

namespace SF.Expressions
{
    /// <summary>
    ///     LambdaExpression Utils
    /// </summary>
    public static class LambdaEx
    {
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
        ///     Returns lambda expression body with parameters repalced with passed arguments
        /// </summary>
        public static Expression Inline(LambdaExpression expression, params Expression[] arguments)
        {
            if (arguments.Length != expression.Parameters.Count)
                throw new ArgumentException("Parameter count does not match");
            if (arguments.Length == 1)
                return ParameterEx.ReplaceParameter(expression.Body, expression.Parameters[0], arguments[0]);
            var lambdaParams = expression.Parameters;
            return ParameterEx.ReplaceParameters(expression.Body, p => arguments[lambdaParams.IndexOf(p)]);
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
        public static LambdaExpression Convert(LambdaExpression lambda, Type delegateType)
        {
            if (lambda.Type == delegateType)
                return lambda;

            var invoke = MethodInfoEx.GetInvokeMethod(delegateType);

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
            var converted = Convert(lambda, typeof (TDelegate));
            return converted.Compile() as TDelegate;
        }
    }
}