using System.IO;
using System.Threading.Tasks;

namespace SF.IO
{
    public static class StreamEx
    {
        public static IDuplexStream FromBlockingStream(Stream stream)
        {
            return new BlockingStreamWrapper(stream);
        }

        public static IDuplexStream FromAsyncStream(Stream stream)
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
    }
}