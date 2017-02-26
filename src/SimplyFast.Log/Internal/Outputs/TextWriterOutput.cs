using System;
using System.IO;
using SimplyFast.Log.Internal.Outputs.Writers;

namespace SimplyFast.Log.Internal.Outputs
{
    public class TextWriterOutput : IOutput, IDisposable
    {
        private readonly TextWriter _textWriter;
        private readonly IWriter _writer;
        private readonly bool _leaveOpen;
        private readonly object _lock = new object();

        public TextWriterOutput(TextWriter textWriter, IWriter writer, bool leaveOpen = false)
        {
            _textWriter = textWriter;
            _writer = writer;
            _leaveOpen = leaveOpen;
        }

        public void Log(IMessage message)
        {
            lock (_lock)
            {
                _writer.Write(_textWriter, message);
                _textWriter.Flush();
            }
        }

        public void Dispose()
        {
            if (!_leaveOpen && _writer != null)
                _textWriter.Dispose();
        }
    }
}