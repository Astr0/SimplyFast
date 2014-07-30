using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    internal class VarIntLengthPrefixedStreamProducer : IProducer<byte[]>
    {
        private readonly byte[] _buffer = new byte[5];
        private readonly Stream _stream;

        public VarIntLengthPrefixedStreamProducer(Stream stream)
        {
            _stream = stream;
        }

        #region IProducer<ArraySegment<byte>> Members

        public async Task Add(byte[] obj, CancellationToken cancellation)
        {
            var count = BufferWriter.WriteVarUInt32(_buffer, 0, (uint) obj.Length);
            await _stream.WriteAsync(_buffer, 0, count, cancellation);
            await _stream.WriteAsync(obj, cancellation);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        #endregion
    }
}