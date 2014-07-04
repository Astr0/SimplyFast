using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    internal class StreamConsumer : IConsumer<ArraySegment<byte>>
    {
        private readonly ArraySegment<byte> _buffer;
        private readonly Stream _stream;

        public StreamConsumer(Stream stream, ArraySegment<byte> buffer)
        {
            _stream = stream;
            _buffer = buffer;
        }

        #region IConsumer<ArraySegment<byte>> Members

        public async Task<ArraySegment<byte>> Take(CancellationToken cancellation)
        {
            var read = await _stream.ReadAsync(_buffer, cancellation);
            if (read == 0)
                throw new EndOfStreamException();
            return new ArraySegment<byte>(_buffer.Array, _buffer.Offset, read);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        #endregion
    }
}