using System.Collections.Generic;
using SimplyFast.Log.Internal.Outputs;

namespace SimplyFast.Log.Internal.Loggers
{
    public class Logger : ILogger
    {
        private readonly string _name;
        private readonly ILogInfo _loggerInfo;
        private Severity _severity;

        public Logger(string name)
        {
            _name = name;
            _loggerInfo = LogInfoEx.Logger(this);
            _severity = Severity.Info;
            // ReSharper disable once VirtualMemberCallInConstructor
            Outputs = new DefaultOutputs();
        }

        public void Log(IMessage message)
        {
            if (!Severity.ShouldLog(message)) return;
            DoLog(message);
        }

        protected virtual void DoLog(IMessage message)
        {
            Outputs.Log(message);
        }

        public IOutputs Outputs { get; }

        public virtual Severity Severity
        {
            get { return _severity; }
            set { _severity = value; }
        }

        public void Log(Severity severity, IEnumerable<ILogInfo> info)
        {
            if (!Severity.ShouldLog(severity))
                return;
            var message = new DefaultMessage(severity)
            {
                _loggerInfo,
                LogInfoEx.Now()
            };
            foreach (var item in info)
            {
                message.Add(item);
            }
        }
        
        public override string ToString()
        {
            return _name;
        }
    }
}