using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SF.IO
{
    public static class StreamEx
    {
        public static Task WriteAsync(this Stream stream, byte[] buffer)
        {
            return stream.WriteAsync(buffer, 0, buffer.Length);
        }

        public static Task WriteAsync(this Stream stream, ArraySegment<byte> buffer)
        {
            return stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count);
        }

        public static Task<int> ReadAsync(this Stream stream, byte[] buffer)
        {
            return stream.ReadAsync(buffer, 0, buffer.Length);
        }

        public static Task<int> ReadAsync(this Stream stream, ArraySegment<byte> buffer)
        {
            return stream.ReadAsync(buffer.Array, buffer.Offset, buffer.Count);
        }

        /// <summary>
        ///     Reads exactly count bytes from stream. Returns number of bytes read.
        /// </summary>
        public static async Task<int> ReadExactAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellation)
        {
            var countRead = 0;
            while (count > 0)
            {
                var read = await stream.ReadAsync(buffer, offset + countRead, count - countRead, cancellation);
                if (read == 0)
                    return countRead;
                countRead += read;
            }
            return countRead;
        }

        public static Task<int> ReadExactAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            return ReadExactAsync(stream, buffer, offset, count, CancellationToken.None);
        }

        public static Task<int> ReadExactAsync(this Stream stream, byte[] buffer, CancellationToken cancellation)
        {
            return ReadExactAsync(stream, buffer, 0, buffer.Length, cancellation);
        }

        public static Task<int> ReadExactAsync(this Stream stream, byte[] buffer)
        {
            return ReadExactAsync(stream, buffer, CancellationToken.None);
        }

        public static Task<int> ReadExactAsync(this Stream stream, ArraySegment<byte> buffer, CancellationToken cancellation)
        {
            return ReadExactAsync(stream, buffer.Array, buffer.Offset, buffer.Count, cancellation);
        }

        public static Task<int> ReadExactAsync(this Stream stream, ArraySegment<byte> buffer)
        {
            return ReadExactAsync(stream, buffer, CancellationToken.None);
        }
    }
}