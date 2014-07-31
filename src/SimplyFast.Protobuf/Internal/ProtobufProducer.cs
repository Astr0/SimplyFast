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
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                var buf = ms.ToArray();
                return _producer.Add(buf, cancellation);
            }
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}