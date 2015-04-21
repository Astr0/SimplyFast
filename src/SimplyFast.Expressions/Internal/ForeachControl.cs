using System.Linq.Expressions;
using SF.Expressions.Dynamic;

namespace SF.Expressions
{
    internal class ForeachControl : IForeachControl
    {
        private readonly ILoopControl _loop;

        public ForeachControl(Expression current, ILoopControl loop)
        {
            _loop = loop;
            Current = current;
        }

        #region IForeachControl Members

        public Expression Break(Expression result = null)
        {
            return _loop.Break(result);
        }

        public Expression Continue()
        {
            return _loop.Continue();
        }

        public Expression Current { get; private set; }

        #endregion
    }
}