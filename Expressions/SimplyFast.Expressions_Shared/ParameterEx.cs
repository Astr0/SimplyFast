using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SF.Expressions
{
    /// <summary>
    ///     ParameterExpression Utils
    /// </summary>
    public static class ParameterEx
    {
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
    }
}