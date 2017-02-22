using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    internal class StreamProducer : IProducer<byte[]>
    {
        private readonly Stream _stream;

        public StreamProducer(Stream stream)
        {
            _stream = stream;
        }

        #region IProducer<ArraySegment<byte>> Members

        public Task Add(byte[] obj, CancellationToken cancellation)
        {
            return _stream.WriteAsync(obj, cancellation);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        #endregion
    }
}