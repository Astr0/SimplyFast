using System;
using System.Linq.Expressions;

namespace SF.Expressions.Dynamic
{
    public static class DynamicEBuilderEx
    {
        #region Expr Builder

        /// <summary>
        /// Creates DynamicEBuilder from expression
        /// </summary>
        public static dynamic EBuilder(this Expression expression)
        {
            return DynamicEBuilder.Create(expression);
        }

        /// <summary>
        /// Create DynamicEBuilder from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static dynamic EBuilder(this Type type)
        {
            return DynamicEBuilder.Create(type);
        }

        #endregion
    }
}