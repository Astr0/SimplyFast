using System;
using System.IO;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    internal class StreamConsumer : IConsumer<ArraySegment<byte>>
    {
        private readonly IInputStream _stream;
        private readonly ArraySegment<byte> _buffer;

        public StreamConsumer(IInputStream stream, ArraySegment<byte> buffer)
        {
            _stream = stream;
            _buffer = buffer;
        }

        public async Task<ArraySegment<byte>> Take()
        {
            var read = await _stream.Read(_buffer);
            if (read == 0)
                throw new EndOfStreamException();
            return new ArraySegment<byte>(_buffer.Array, _buffer.Offset, read);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}