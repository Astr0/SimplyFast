using System.Collections.Generic;

namespace SimplyFast.Log
{
    public interface ILogger : IOutput
    {
        IOutputs Outputs { get; }
        Severity Severity { get; set; }
        /// <summary>
        /// Log message
        /// </summary>
        void Log(Severity severity, IEnumerable<ILogInfo> info);
    }
}