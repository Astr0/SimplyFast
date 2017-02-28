using System;

namespace SimplyFast.Log
{
    /// <summary>
    ///     Log writer
    /// </summary>
    public interface IOutput: IDisposable
    {
        /// <summary>
        ///     Log passed message
        /// </summary>
        void Log(IMessage message);
    }
}