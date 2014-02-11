using System;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    internal class IntLengthPrefixedStreamProducer : IProducer<ArraySegment<byte>>
    {
        private readonly IOutputStream _stream;

        public IntLengthPrefixedStreamProducer(IOutputStream stream)
        {
            _stream = stream;
        }

        public async Task Add(ArraySegment<byte> obj)
        {
            await _stream.Write(BitConverter.GetBytes(obj.Count));
            await _stream.Write(obj);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}