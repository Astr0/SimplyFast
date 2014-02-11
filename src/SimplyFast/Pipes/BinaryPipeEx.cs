using System;
using SF.IO;

namespace SF.Pipes
{
    public static class BinaryPipeEx
    {
        /// <summary>
        ///     No framing, just sends bytes to stream
        /// </summary>
        public static IProducer<ArraySegment<byte>> AsBinaryProducer(this IOutputStream stream)
        {
            return new StreamProducer(stream);
        }

        /// <summary>
        ///     No framing, just reads bytes from stream
        /// </summary>
        public static IConsumer<ArraySegment<byte>> AsBinaryConsumer(this IInputStream stream, ArraySegment<byte> buffer)
        {
            return new StreamConsumer(stream, buffer);
        }

        /// <summary>
        ///     No framing, just reads bytes from stream
        /// </summary>
        public static IConsumer<ArraySegment<byte>> AsBinaryConsumer(this IInputStream stream, int bufferSize = 256)
        {
            return new StreamConsumer(stream, new ArraySegment<byte>(new byte[bufferSize]));
        }

        public static IProducer<ArraySegment<byte>> AsIntLengthPrefixedProducer(this IOutputStream stream)
        {
            return new IntLengthPrefixedStreamProducer(stream);
        }

        public static IConsumer<ArraySegment<byte>> AsIntLengthPrefixedConsumer(this IInputStream stream, int bufferCapacity = 256)
        {
            return new IntLengthPrefixedStreamConsumer(stream, bufferCapacity);
        }

        public static IProducer<ArraySegment<byte>> AsVarIntLengthPrefixedProducer(this IOutputStream stream)
        {
            return new VarIntLengthPrefixedStreamProducer(stream);
        }

        public static IConsumer<ArraySegment<byte>> AsVarIntLengthPrefixedConsumer(this IInputStream stream, int bufferCapacity = 256)
        {
            return new VarIntLengthPrefixedStreamConsumer(stream, bufferCapacity);
        }
    }
}