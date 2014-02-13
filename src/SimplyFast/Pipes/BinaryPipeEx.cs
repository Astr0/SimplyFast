using System;
using System.IO;

namespace SF.Pipes
{
    public static class BinaryPipeEx
    {
        /// <summary>
        ///     No framing, just sends bytes to stream
        /// </summary>
        public static IProducer<ArraySegment<byte>> AsBinaryProducer(this Stream stream)
        {
            return new StreamProducer(stream);
        }

        /// <summary>
        ///     No framing, just reads bytes from stream
        /// </summary>
        public static IConsumer<ArraySegment<byte>> AsBinaryConsumer(this Stream stream, ArraySegment<byte> buffer)
        {
            return new StreamConsumer(stream, buffer);
        }

        /// <summary>
        ///     No framing, just reads bytes from stream
        /// </summary>
        public static IConsumer<ArraySegment<byte>> AsBinaryConsumer(this Stream stream, int bufferSize = 256)
        {
            return new StreamConsumer(stream, new ArraySegment<byte>(new byte[bufferSize]));
        }

        public static IProducer<ArraySegment<byte>> AsIntLengthPrefixedProducer(this Stream stream)
        {
            return new IntLengthPrefixedStreamProducer(stream);
        }

        public static IConsumer<ArraySegment<byte>> AsIntLengthPrefixedConsumer(this Stream stream, int bufferCapacity = 256)
        {
            return new IntLengthPrefixedStreamConsumer(stream, bufferCapacity);
        }

        public static IProducer<ArraySegment<byte>> AsVarIntLengthPrefixedProducer(this Stream stream)
        {
            return new VarIntLengthPrefixedStreamProducer(stream);
        }

        public static IConsumer<ArraySegment<byte>> AsVarIntLengthPrefixedConsumer(this Stream stream, int bufferCapacity = 256)
        {
            return new VarIntLengthPrefixedStreamConsumer(stream, bufferCapacity);
        }
    }
}