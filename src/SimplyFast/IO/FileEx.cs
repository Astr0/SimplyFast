using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SF.IO
{
    public static class FileEx
    {
        public static async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken token = default (CancellationToken))
        {
            byte[] buffer;
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None))
            {
                var offset = 0;
                var length = fileStream.Length;
                var maxLength = (long)int.MaxValue;
                if (length > maxLength)
                    throw new IOException("File greater than 2Gb");
                var count = (int)length;
                buffer = new byte[count];
                while (count > 0)
                {
                    var read = await fileStream.ReadAsync(buffer, offset, count, token).ConfigureAwait(false);
                    if (read == 0)
                        throw new IOException("End of file");
                    offset += read;
                    count -= read;
                }
            }
            return buffer;
    }

        public static async Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken token = default(CancellationToken))
        {
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.None))
                await fileStream.WriteAsync(bytes, 0, bytes.Length, token);
        }
    }
}