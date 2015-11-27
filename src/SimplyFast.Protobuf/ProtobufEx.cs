using System.IO;
using ProtoBuf;
using SF.Pipes;

namespace SF.Protobuf
{
    public static class ProtobufEx
    {
        public static IConsumer<T> ProtobufDeserializer<T>(this IConsumer<byte[]> consumer)
        {
            return consumer.Select(Deserialize<T>);
        }

        public static IProducer<T> ProtobufSerializer<T>(this IProducer<byte[]> producer)
        {
            return producer.Convert<byte[], T>(Serialize);
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