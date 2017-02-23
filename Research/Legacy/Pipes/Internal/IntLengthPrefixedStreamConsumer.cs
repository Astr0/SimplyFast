using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SF.Collections;
using SF.IO;

namespace SF.Pipes
{
    internal class IntLengthPrefixedStreamConsumer : IConsumer<byte[]>
    {
        private readonly Stream _stream;
        private readonly byte[] _buffer;

        public IntLengthPrefixedStreamConsumer(Stream stream)
        {
            _stream = stream;
            _buffer = new byte[4];
        }

        #region IConsumer<ArraySegment<byte>> Members

        public async Task<byte[]> Take(CancellationToken cancellation)
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
                return TypeHelper<byte>.EmptyArray;

            var result = new byte[length];

            // read message
            crc = await _stream.ReadExactAsync(result, 0, length, cancellation);

            // check if not EOF
            if (crc != length)
                throw new EndOfStreamException();

            // return message
            return result;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        #endregion
    }
}