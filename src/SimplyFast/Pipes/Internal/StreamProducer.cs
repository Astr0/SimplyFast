using System;
using System.IO;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    internal class StreamProducer : IProducer<ArraySegment<byte>>
    {
        private readonly Stream _stream;

        public StreamProducer(Stream stream)
        {
            _stream = stream;
        }

        public Task Add(ArraySegment<byte> obj)
        {
            return _stream.WriteAsync(obj);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}