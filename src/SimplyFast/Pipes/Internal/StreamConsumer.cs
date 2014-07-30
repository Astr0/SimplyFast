using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    internal class StreamConsumer : IConsumer<byte[]>
    {
        private readonly byte[] _buffer;
        private readonly Stream _stream;

        public StreamConsumer(Stream stream, byte[] buffer)
        {
            _stream = stream;
            _buffer = buffer;
        }

        #region IConsumer<ArraySegment<byte>> Members

        public async Task<byte[]> Take(CancellationToken cancellation)
        {
            var read = await _stream.ReadAsync(_buffer, cancellation);
            if (read == 0)
                throw new EndOfStreamException();
            if (read == _buffer.Length) 
                return _buffer;
            var res = new byte[read];
            Array.Copy(_buffer, res, read);
            return res;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        #endregion
    }
}