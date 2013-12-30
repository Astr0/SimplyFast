using System.Linq.Expressions;

namespace SF.Expressions
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