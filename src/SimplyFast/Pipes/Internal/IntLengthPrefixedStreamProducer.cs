using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    internal class IntLengthPrefixedStreamProducer : IProducer<byte[]>
    {
        private readonly Stream _stream;

        public IntLengthPrefixedStreamProducer(Stream stream)
        {
            _stream = stream;
        }

        #region IProducer<ArraySegment<byte>> Members

        public async Task Add(byte[] obj, CancellationToken cancellation)
        {
            await _stream.WriteAsync(BitConverter.GetBytes(obj.Length), cancellation);
            await _stream.WriteAsync(obj, cancellation);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        #endregion
    }
}