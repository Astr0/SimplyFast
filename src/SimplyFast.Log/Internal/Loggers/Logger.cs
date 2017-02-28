using System;
using System.Collections.Generic;
using SimplyFast.Log.Internal.Outputs;
using SimplyFast.Strings.Tokens;

namespace SimplyFast.Log.Internal.Loggers
{
    internal class Logger : ILogger
    {
        private readonly IStringToken _loggerInfo;
        private readonly Func<Severity, IMessage> _messageFactory;
        private readonly string _name;
        private Severity _severity;

        public Logger(string name, Func<Severity, IMessage> messageFactory)
        {
            _name = name;
            _messageFactory = messageFactory;
            _loggerInfo = LogTokenEx.Logger(this);
            _severity = Severity.Info;
            // ReSharper disable once VirtualMemberCallInConstructor
            Outputs = new DefaultOutputs();
        }

        public void Log(IMessage message)
        {
            if (!Severity.ShouldLog(message)) return;
            DoLog(message);
        }

        public IOutputs Outputs { get; }

        public virtual Severity Severity
        {
            get { return _severity; }
            set { _severity = value; }
        }

        public void Log(Severity severity, IEnumerable<IStringToken> info)
        {
            if (!Severity.ShouldLog(severity))
                return;
            var message = _messageFactory(severity);
            message.Add(_loggerInfo);
            message.Add(LogTokenEx.Now());
            foreach (var item in info)
                message.Add(item);
        }

        protected virtual void DoLog(IMessage message)
        {
            Outputs.Log(message);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}