using System.Linq.Expressions;

namespace SimplyFast.Expressions
{
    /// <summary>
    ///     Controls loop expression building
    /// </summary>
    public interface ILoopControl
    {
        Expression Break(Expression result = null);
        Expression Continue();
    }
}