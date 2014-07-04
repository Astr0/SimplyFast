using System;
using System.IO;

namespace SF.Pipes
{
    /// <summary>
    /// Utils to convert Stream to a pipe
    /// </summary>
    public static class StreamPipeEx
    {
        /// <summary>
        ///     Converts Stream to IProducer with no framing
        /// </summary>
        public static IProducer<ArraySegment<byte>> AsBinaryProducer(this Stream stream)
        {
            return new StreamProducer(stream);
        }

        /// <summary>
        ///     Converts Stream to IConsumer with no framing and fixed buffer
        /// </summary>
        public static IConsumer<ArraySegment<byte>> AsBinaryConsumer(this Stream stream, ArraySegment<byte> buffer)
        {
            return new StreamConsumer(stream, buffer);
        }

        /// <summary>
        ///     Converts Stream to IProducer with no framing and fixed buffer size
        /// </summary>
        public static IConsumer<ArraySegment<byte>> AsBinaryConsumer(this Stream stream, int bufferSize = 256)
        {
            return new StreamConsumer(stream, new ArraySegment<byte>(new byte[bufferSize]));
        }

        /// <summary>
        ///     Converts Stream to IProducer with Integer length framing
        /// </summary>
        public static IProducer<ArraySegment<byte>> AsIntLengthPrefixedProducer(this Stream stream)
        {
            return new IntLengthPrefixedStreamProducer(stream);
        }

        /// <summary>
        ///     Converts Stream to IConsumer with Integer length framing
        /// </summary>
        public static IConsumer<ArraySegment<byte>> AsIntLengthPrefixedConsumer(this Stream stream, int bufferCapacity = 256)
        {
            return new IntLengthPrefixedStreamConsumer(stream, bufferCapacity);
        }

        /// <summary>
        ///     Converts Stream to IProducer with VarInt length framing
        /// </summary>
        public static IProducer<ArraySegment<byte>> AsVarIntLengthPrefixedProducer(this Stream stream)
        {
            return new VarIntLengthPrefixedStreamProducer(stream);
        }

        /// <summary>
        ///     Converts Stream to IConsumer with VarInt length framing
        /// </summary>
        public static IConsumer<ArraySegment<byte>> AsVarIntLengthPrefixedConsumer(this Stream stream, int bufferCapacity = 256)
        {
            return new VarIntLengthPrefixedStreamConsumer(stream, bufferCapacity);
        }
    }
}