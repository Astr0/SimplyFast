using SimplyFast.Strings.Tokens;

namespace SimplyFast.Log
{
    /// <summary>
    ///     Interface for log message
    /// </summary>
    public interface IMessage : IStringTokenStorage
    {
        /// <summary>
        ///     Message severity
        /// </summary>
        Severity Severity { get; }
    }
}