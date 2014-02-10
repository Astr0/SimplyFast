using System.IO;
using System.Threading.Tasks;

namespace SF.IO
{
    public class AsyncStreamWrapper : IInputStream, IOutputStream
    {
        private readonly Stream _stream;

        public AsyncStreamWrapper(Stream stream)
        {
            _stream = stream;
        }

        #region IInputStream Members

        public void Dispose()
        {
            _stream.Dispose();
        }

        public Task<int> Read(byte[] buffer, int offset, int count)
        {
            return _stream.ReadAsync(buffer, offset, count);
        }

        #endregion

        #region IOutputStream Members

        public Task Write(byte[] buffer, int offset, int count)
        {
            return _stream.WriteAsync(buffer, offset, count);
        }

        #endregion
    }
}