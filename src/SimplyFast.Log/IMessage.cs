namespace SimplyFast.Log
{
    /// <summary>
    /// Interface for log message
    /// </summary>
    public interface IMessage: ILogInfoStorage
    {
        /// <summary>
        /// Message severity
        /// </summary>
        Severity Severity { get; }
    }
}