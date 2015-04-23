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
        public static IProducer<byte[]> AsBinaryProducer(this Stream stream)
        {
            return new StreamProducer(stream);
        }

        /// <summary>
        ///     Converts Stream to IConsumer with no framing and fixed buffer
        /// </summary>
        public static IConsumer<byte[]> AsBinaryConsumer(this Stream stream, byte[] buffer = null)
        {
            return new StreamConsumer(stream, buffer ?? new byte[256]);
        }

        /// <summary>
        ///     Converts Stream to IProducer with Integer length framing
        /// </summary>
        public static IProducer<byte[]> AsIntLengthPrefixedProducer(this Stream stream)
        {
            return new IntLengthPrefixedStreamProducer(stream);
        }

        /// <summary>
        ///     Converts Stream to IConsumer with Integer length framing
        /// </summary>
        public static IConsumer<byte[]> AsIntLengthPrefixedConsumer(this Stream stream)
        {
            return new IntLengthPrefixedStreamConsumer(stream);
        }

        /// <summary>
        ///     Converts Stream to IProducer with VarInt length framing
        /// </summary>
        public static IProducer<byte[]> AsVarIntLengthPrefixedProducer(this Stream stream)
        {
            return new VarIntLengthPrefixedStreamProducer(stream);
        }

        /// <summary>
        ///     Converts Stream to IConsumer with VarInt length framing
        /// </summary>
        public static IConsumer<byte[]> AsVarIntLengthPrefixedConsumer(this Stream stream, int bufferCapacity = 256)
        {
            return new VarIntLengthPrefixedStreamConsumer(stream, bufferCapacity);
        }
    }
}