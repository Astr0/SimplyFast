using System;
using System.IO;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    internal class IntLengthPrefixedStreamProducer : IProducer<ArraySegment<byte>>
    {
        private readonly Stream _stream;

        public IntLengthPrefixedStreamProducer(Stream stream)
        {
            _stream = stream;
        }

        public async Task Add(ArraySegment<byte> obj)
        {
            await _stream.WriteAsync(BitConverter.GetBytes(obj.Count));
            await _stream.WriteAsync(obj);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}