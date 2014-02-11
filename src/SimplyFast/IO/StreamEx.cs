using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SF.IO
{
    public static class StreamEx
    {
        public static BlockingStreamWrapper FromBlockingStream(Stream stream)
        {
            return new BlockingStreamWrapper(stream);
        }

        public static AsyncStreamWrapper FromAsyncStream(Stream stream)
        {
            return new AsyncStreamWrapper(stream);
        }

        public static Task Write(this IOutputStream stream, byte[] buffer)
        {
            return stream.Write(buffer, 0, buffer.Length);
        }

        public static Task Write(this IOutputStream stream, ArraySegment<byte> buffer)
        {
            return stream.Write(buffer.Array, buffer.Offset, buffer.Count);
        }

        public static Task<int> Read(this IInputStream stream, byte[] buffer)
        {
            return stream.Read(buffer, 0, buffer.Length);
        }

        public static Task<int> Read(this IInputStream stream, ArraySegment<byte> buffer)
        {
            return stream.Read(buffer.Array, buffer.Offset, buffer.Count);
        }

        /// <summary>
        /// Reads exactly count bytes from stream. Returns number of bytes read. 
        /// </summary>
        public static async Task<int> ReadExact(this IInputStream stream, byte[] buffer, int offset, int count, CancellationToken cancellation)
        {
            var countRead = 0;
            while (count > 0)
            {
                cancellation.ThrowIfCancellationRequested();
                var read = await stream.Read(buffer, offset + countRead, count - countRead);
                if (read == 0)
                    return countRead;
                countRead += read;
            }
            return countRead;
        }

        public static Task<int> ReadExact(this IInputStream stream, byte[] buffer, int offset, int count)
        {
            return ReadExact(stream, buffer, offset, count, CancellationToken.None);
        }

        public static Task<int> ReadExact(this IInputStream stream, byte[] buffer, CancellationToken cancellation)
        {
            return ReadExact(stream, buffer, 0, buffer.Length, cancellation);
        }

        public static Task<int> ReadExact(this IInputStream stream, byte[] buffer)
        {
            return ReadExact(stream, buffer, CancellationToken.None);
        }

        public static Task<int> ReadExact(this IInputStream stream, ArraySegment<byte> buffer, CancellationToken cancellation)
        {
            return ReadExact(stream, buffer.Array, buffer.Offset, buffer.Count, cancellation);
        }

        public static Task<int> ReadExact(this IInputStream stream, ArraySegment<byte> buffer)
        {
            return ReadExact(stream, buffer, CancellationToken.None);
        }

        public static async Task CopyTo(this IInputStream input, IOutputStream target, CancellationToken cancellation, int bufferSize = 256)
        {
            cancellation.ThrowIfCancellationRequested();
            var buffer = new byte[bufferSize];
            var read = await input.Read(buffer, 0, bufferSize);
            while (read > 0)
            {
                await target.Write(buffer, 0, read);
                cancellation.ThrowIfCancellationRequested();
                read = await input.Read(buffer, 0, bufferSize);
            }
        }

        public static Task CopyTo(this IInputStream input, IOutputStream target, int bufferSize = 256)
        {
            return CopyTo(input, target, CancellationToken.None, bufferSize);
        }
    }
}