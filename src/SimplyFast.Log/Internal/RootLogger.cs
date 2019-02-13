using SimplyFast.Log.Messages;

namespace SimplyFast.Log.Internal
{
    internal class RootLogger : ILogger
    {
        private volatile Severity _severity = Severity.Info;

        public RootLogger(IMessageFactory messageFactory, string name = null)
        {
            MessageFactory = messageFactory;
            Name = name ?? "Root";
        }

        public Severity Severity
        {
            get => _severity;
            set => _severity = value;
        }

        public IMessageFactory MessageFactory { get; }

        public void Log(IMessage message)
        {
            if (!_severity.ShouldLog(message))
                return;
            Outputs.Log(message);
        }

        public string Name { get; }

        public IOutputs Outputs { get; } = new OutputCollection();

        public override string ToString()
        {
            return Name;
        }
    }
}