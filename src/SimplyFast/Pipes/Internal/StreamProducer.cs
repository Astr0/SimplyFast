using System;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    internal class StreamProducer : IProducer<ArraySegment<byte>>
    {
        private readonly IOutputStream _stream;

        public StreamProducer(IOutputStream stream)
        {
            _stream = stream;
        }

        public Task Add(ArraySegment<byte> obj)
        {
            return _stream.Write(obj);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}