using System.Linq.Expressions;

namespace SF.Expressions
{
    /// <summary>
    ///     Controls foreach expression building
    /// </summary>
    public interface IForeachControl : ILoopControl
    {
        Expression Current { get; }
    }
}