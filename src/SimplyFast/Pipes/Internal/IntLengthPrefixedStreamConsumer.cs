using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    internal class IntLengthPrefixedStreamConsumer : IConsumer<ArraySegment<byte>>
    {
        private readonly Stream _stream;
        private byte[] _buffer;

        public IntLengthPrefixedStreamConsumer(Stream stream, int bufferCapacity)
        {
            _stream = stream;
            _buffer = new byte[Math.Min(4, bufferCapacity)];
        }

        #region IConsumer<ArraySegment<byte>> Members

        public async Task<ArraySegment<byte>> Take(CancellationToken cancellation)
        {
            // read length
            var crc = await _stream.ReadExactAsync(_buffer, 0, 4, cancellation);
            // check if not EOF
            if (crc != 4)
                throw new EndOfStreamException();

            // decode length
            var length = BitConverter.ToInt32(_buffer, 0);

            // perf optimization - don't read if length == 0
            if (length == 0)
                return new ArraySegment<byte>(_buffer, 0, 0);

            // check buffer capacity
            if (_buffer.Length < length)
                _buffer = new byte[length];

            // read message
            crc = await _stream.ReadExactAsync(_buffer, 0, length, cancellation);

            // check if not EOF
            if (crc != length)
                throw new EndOfStreamException();

            // return message
            return new ArraySegment<byte>(_buffer, 0, length);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        #endregion
    }
}