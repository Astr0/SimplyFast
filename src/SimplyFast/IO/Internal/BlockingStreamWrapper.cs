using System.IO;
using System.Threading.Tasks;
using SF.Threading;

namespace SF.IO
{
    internal class BlockingStreamWrapper : IDuplexStream
    {
        private readonly Stream _stream;

        public BlockingStreamWrapper(Stream stream)
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
            _stream.Write(buffer, offset, count);
            return TaskEx.Completed;
        }

        public Task<int> Read(byte[] buffer, int offset, int count)
        {
            return Task.FromResult(_stream.Read(buffer, offset, count));
        }

        #endregion
    }
}