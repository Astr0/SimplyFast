using System.IO;
using ProtoBuf;
using SF.Pipes;

namespace SF.Protobuf
{
    public static class ProtobufEx
    {
        public static IConsumer<T> AsProtobufConsumer<T>(this IConsumer<byte[]> consumer)
        {
            return new ProtobufConsumer<T>(consumer);
        }

        public static IProducer<T> AsProtobufProducer<T>(this IProducer<byte[]> producer)
        {
            return new ProtobufProducer<T>(producer);
        }

        public static T Deserialize<T>(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        public static byte[] Serialize<T>(T instance)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, instance);
                return ms.ToArray();
            }
        }
    }
}