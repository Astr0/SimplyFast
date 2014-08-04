using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf;
using SF.Pipes;

namespace SF.Protobuf
{
    internal class ProtobufProducer<T>: IProducer<T>
    {
        private readonly IProducer<byte[]> _producer;

        public ProtobufProducer(IProducer<byte[]> producer)
        {
            _producer = producer;
        }

        public Task Add(T obj, CancellationToken cancellation = new CancellationToken())
        {
            return _producer.Add(ProtobufEx.Serialize(obj), cancellation);
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}