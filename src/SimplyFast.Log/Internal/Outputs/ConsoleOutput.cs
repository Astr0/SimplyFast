
#if CONSOLE
using System;

namespace SimplyFast.Log.Internal.Outputs
{
    internal class ConsoleOutput : IOutput
    {
        private readonly TextWriterOutput _output;
        private readonly TextWriterOutput _error;

        public ConsoleOutput(IWriter writer)
        {
            _output = new TextWriterOutput(Console.Out, writer, true);
            _error = new TextWriterOutput(Console.Error, writer, true);
        }

        public void Log(IMessage message)
        {
            var target = Severity.Error.ShouldLog(message) ? _error : _output;
            target.Log(message);
        }

        public void Dispose()
        {
            _output.Dispose();
            _error.Dispose();
        }
    }
}
#endif