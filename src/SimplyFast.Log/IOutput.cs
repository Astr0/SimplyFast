namespace SimplyFast.Log
{
    /// <summary>
    ///     Log writer
    /// </summary>
    public interface IOutput
    {
        /// <summary>
        ///     Log passed message
        /// </summary>
        void Log(IMessage message);
    }
}