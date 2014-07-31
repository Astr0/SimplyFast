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
    }
}