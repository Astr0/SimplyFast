using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SF.Collections;
using SF.Threading;

namespace SF.IO
{
    /// <summary>
    /// Stream utilities
    /// </summary>
    public static class StreamEx
    {
        /// <summary>
        /// Async Writes full buffer to a stream
        /// </summary>
        public static Task WriteAsync(this Stream stream, byte[] buffer, CancellationToken token = default(CancellationToken))
        {
            return stream.WriteAsync(buffer, 0, buffer.Length, token);
        }

        /// <summary>
        /// Async writes array segment to a stream
        /// </summary>
        public static Task WriteAsync(this Stream stream, ArraySegment<byte> buffer, CancellationToken token = default(CancellationToken))
        {
            return stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count, token);
        }

        /// <summary>
        /// Async writes byte collection to a stream
        /// </summary>
        public static Task WriteAsync(this Stream stream, IReadOnlyCollection<byte> collection, CancellationToken token = default(CancellationToken))
        {
            if (collection is ArraySegment<byte>)
                return WriteAsync(stream, (ArraySegment<byte>) collection, token);
            var array = collection as byte[];
            if (array != null)
                return WriteAsync(stream, array, token);
            var buffer = new byte[collection.Count];
            collection.CopyTo(buffer);
            return stream.WriteAsync(buffer, token);
        }

        /// <summary>
        /// Reads async buffer from a stream
        /// </summary>
        public static Task<int> ReadAsync(this Stream stream, byte[] buffer, CancellationToken token = default (CancellationToken))
        {
            return stream.ReadAsync(buffer, 0, buffer.Length, token);
        }

        /// <summary>
        /// Reads async ArraySegment of bytes from a stream
        /// </summary>
        public static Task<int> ReadAsync(this Stream stream, ArraySegment<byte> buffer, CancellationToken token = default (CancellationToken))
        {
            return stream.ReadAsync(buffer.Array, buffer.Offset, buffer.Count, token);
        }

        /// <summary>
        ///     Reads exactly count bytes from stream. Returns number of bytes read.
        /// </summary>
        public static async Task<int> ReadExactAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellation = default(CancellationToken))
        {
            var countRead = 0;
            while (countRead < count)
            {
                var read = await stream.ReadAsync(buffer, offset + countRead, count - countRead, cancellation);
                if (read == 0)
                    return countRead;
                countRead += read;
            }
            return countRead;
        }

        /// <summary>
        ///     Reads exactly count bytes from stream. Returns number of bytes read.
        /// </summary>
        public static Task<int> ReadExactAsync(this Stream stream, byte[] buffer, CancellationToken cancellation = default (CancellationToken))
        {
            return ReadExactAsync(stream, buffer, 0, buffer.Length, cancellation);
        }

        /// <summary>
        ///     Reads exactly count bytes from stream. Returns number of bytes read.
        /// </summary>
        public static Task<int> ReadExactAsync(this Stream stream, ArraySegment<byte> buffer, CancellationToken cancellation = default (CancellationToken))
        {
            return ReadExactAsync(stream, buffer.Array, buffer.Offset, buffer.Count, cancellation);
        }

        /// <summary>
        /// Reads exactly count of bytes from a stream. Returns data read
        /// </summary>
        public static Task<byte[]> ReadExactAsync(this Stream stream, int count, CancellationToken cancellation = default (CancellationToken))
        {
            var buffer = new byte[count];
            return ReadExactAsync(stream, buffer, cancellation).Then(read =>
            {
                if (read < count)
                    Array.Resize(ref buffer, read);
                return buffer;
            });
        }
    }
}