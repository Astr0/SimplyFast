using System;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    internal class VarIntLengthPrefixedStreamProducer : IProducer<ArraySegment<byte>>
    {
        private readonly byte[] _buffer = new byte[5];
        private readonly IOutputStream _stream;

        public VarIntLengthPrefixedStreamProducer(IOutputStream stream)
        {
            _stream = stream;
        }

        public async Task Add(ArraySegment<byte> obj)
        {
            var count = BufferWriter.WriteVarUInt32(_buffer, 0, (uint) obj.Count);
            await _stream.Write(_buffer, 0, count);
            await _stream.Write(obj);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}