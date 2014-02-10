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

        public static Task<int> Read(this IInputStream stream, byte[] buffer)
        {
            return stream.Read(buffer, 0, buffer.Length);
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