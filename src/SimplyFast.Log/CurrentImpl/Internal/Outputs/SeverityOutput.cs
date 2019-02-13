namespace SimplyFast.Log.Internal.Outputs
{
    internal class SeverityOutput : IOutput
    {
        private readonly IOutput _output;
        private readonly Severity _severity;

        public SeverityOutput(Severity severity, IOutput output)
        {
            _severity = severity;
            _output = output;
        }

        public void Log(IMessage message)
        {
            if (_severity.ShouldLog(message))
                _output.Log(message);
        }

        public void Dispose()
        {
            _output?.Dispose();
        }
    }
}