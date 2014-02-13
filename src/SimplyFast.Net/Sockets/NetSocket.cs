using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using SF.Pool;

namespace SF.Net.Sockets
{
    public class NetSocket : ISocket
    {
        private readonly NetStream _stream;

        public NetSocket(Socket socket, IPool<SocketAsyncEventArgs> pool)
        {
            _stream = new NetStream(socket, pool);
        }

        #region ISocket Members

        public Stream Stream
        {
            get { return _stream; }
        }

        public Task Disconnect()
        {
            return _stream.Disconnect();
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        #endregion

        public override string ToString()
        {
            return _stream.SocketName;
        }
    }
}