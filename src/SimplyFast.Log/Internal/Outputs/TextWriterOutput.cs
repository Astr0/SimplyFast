using System.IO;

namespace SimplyFast.Log.Internal.Outputs
{
    internal class TextWriterOutput : IOutput
    {
        private readonly bool _leaveOpen;
        private readonly object _lock = new object();
        private readonly TextWriter _textWriter;
        private readonly IWriter _writer;

        public TextWriterOutput(TextWriter textWriter, IWriter writer, bool leaveOpen = false)
        {
            _textWriter = textWriter;
            _writer = writer;
            _leaveOpen = leaveOpen;
        }

        public void Dispose()
        {
            if (!_leaveOpen && _writer != null)
                _textWriter.Dispose();
        }

        public void Log(IMessage message)
        {
            lock (_lock)
            {
                _writer.Write(_textWriter, message);
                _textWriter.Flush();
            }
        }
    }
}