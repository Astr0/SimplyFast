using SimplyFast.Log.Messages;

namespace SimplyFast.Log.Internal
{
    internal class SeverityOutput : IOutput
    {
        private readonly Severity _severity;
        private readonly IOutput _target;

        public SeverityOutput(IOutput target, Severity severity)
        {
            _target = target;
            _severity = severity;
        }

        public void Log(IMessage message)
        {
            if (_severity.ShouldLog(message))
                _target.Log(message);
        }

        public void Dispose()
        {
            _target.Dispose();
        }
    }
}