using System.Collections.Generic;
using SimplyFast.Strings.Tokens;

namespace SimplyFast.Log
{
    public interface ILogger : IOutput
    {
        IOutputs Outputs { get; }
        Severity Severity { get; set; }

        /// <summary>
        ///     Log message
        /// </summary>
        void Log(Severity severity, IEnumerable<IStringToken> info);
    }
}