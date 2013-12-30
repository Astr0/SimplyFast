using System;
using System.Linq.Expressions;
using SF.Reflection;

namespace SF.Expressions
{
    /// <summary>
    ///     Expression Utils
    /// </summary>
    public static class ExpressionEx
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