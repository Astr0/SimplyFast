using System;
using SF.IO;

namespace SF.Pipes
{
    public static class BinaryPipeEx
    {
        /// <summary>
        ///     No framing, just sends bytes to stream
        /// </summary>
        public static IProducer<ArraySegment<byte>> AsBinaryPipe(this IOutputStream stream)
        {
            return new StreamProducer(stream);
        }

        /// <summary>
        ///     No framing, just reads bytes from stream
        /// </summary>
        public static IConsumer<ArraySegment<byte>> AsBinaryPipe(this IInputStream stream, ArraySegment<byte> buffer)
        {
            return new StreamConsumer(stream, buffer);
        }

        /// <summary>
        ///     No framing, just reads bytes from stream
        /// </summary>
        public static IConsumer<ArraySegment<byte>> AsBinaryPipe(this IInputStream stream, int bufferSize = 256)
        {
            return new StreamConsumer(stream, new ArraySegment<byte>(new byte[bufferSize]));
        }
    }
}