namespace SimplyFast.Log.Internal.Outputs
{
    public class SeverityOutput: IOutput
    {
        private readonly Severity _severity;
        private readonly IOutput _output;

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
    }
}