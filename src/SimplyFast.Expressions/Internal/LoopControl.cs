using System.Linq.Expressions;
using SF.Expressions.Dynamic;

namespace SF.Expressions
{
    internal class LoopControl : ILoopControl
    {
        internal LabelTarget BreakLabel;
        internal LabelTarget ContinueLabel;

        #region ILoopControl Members

        public Expression Break(Expression result = null)
        {
            if (BreakLabel == null)
            {
                BreakLabel = result == null ? Expression.Label() : Expression.Label(result.Type);
            }
            return result == null ? Expression.Break(BreakLabel) : Expression.Break(BreakLabel, result);
        }

        public Expression Continue()
        {
            if (ContinueLabel == null)
                ContinueLabel = Expression.Label();
            return Expression.Continue(ContinueLabel);
        }

        #endregion
    }
}