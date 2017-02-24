using System.Linq.Expressions;

namespace SimplyFast.Expressions
{
    /// <summary>
    ///     Controls foreach expression building
    /// </summary>
    public interface IForeachControl : ILoopControl
    {
        Expression Current { get; }
    }
}