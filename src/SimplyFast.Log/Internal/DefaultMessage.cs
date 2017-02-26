using System.Collections.Generic;
using SimplyFast.Log.Internal.InfoProviders;

namespace SimplyFast.Log.Internal
{
    internal class DefaultMessage : ListLogInfoStorage, IMessage
    {
        public DefaultMessage(Severity severity)
            : base(new List<ILogInfo>
            {
                LogInfoEx.Severity(severity)
            })
        {
            Severity = severity;
        }

        public Severity Severity { get; }
    }
}