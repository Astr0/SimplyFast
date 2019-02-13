using SimplyFast.Log.Messages;

namespace SimplyFast.Log.Internal
{
    internal class ChildLogger : ILogger
    {
        private readonly ILogger _parent;
        private Severity? _severity;

        public ChildLogger(string name, ILogger parent)
        {
            Name = name;
            _parent = parent;
        }

        public Severity Severity
        {
            get => _severity ?? _parent.Severity;
            set => _severity = value;
        }

        public void Log(IMessage message)
        {
            if (!Severity.ShouldLog(message))
                return;

            Outputs.Log(message);
            _parent.Log(message);
        }

        public IOutputs Outputs { get; } = new OutputCollection();

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}