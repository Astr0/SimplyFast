
#if CONSOLE
using System;
using System.IO;

namespace SimplyFast.Log.Internal.Outputs
{
    internal class ConsoleOutput : IOutput, IDisposable
    {
        private readonly IWriter _writer;
        private readonly TextWriterOutput _output;
        private readonly TextWriterOutput _error;

        private TextWriterOutput CreateConsoleStreamOutput(Stream consoleStream)
        {
            var streamWriter = new StreamWriter(consoleStream, Console.OutputEncoding, 256, true);
            return new TextWriterOutput(streamWriter, _writer, true);

        }

        public ConsoleOutput(IWriter writer)
        {
            _writer = writer;
            _output = CreateConsoleStreamOutput(Console.OpenStandardOutput());
            _error = CreateConsoleStreamOutput(Console.OpenStandardError());
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