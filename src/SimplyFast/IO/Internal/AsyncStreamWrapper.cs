using System.IO;
using System.Threading.Tasks;

namespace SF.IO
{
    internal class AsyncStreamWrapper : IDuplexStream
    {
        private readonly Stream _stream;

        public AsyncStreamWrapper(Stream stream)
        {
            _stream = stream;
        }

        #region IDuplexStream Members

        public void Dispose()
        {
            _stream.Dispose();
        }

        public Task Write(byte[] buffer, int offset, int count)
        {
            return _stream.WriteAsync(buffer, offset, count);
        }

        public Task<int> Read(byte[] buffer, int offset, int count)
        {
            return _stream.ReadAsync(buffer, offset, count);
        }

        #endregion
    }
}