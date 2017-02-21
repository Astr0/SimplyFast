using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SF.Pool;

namespace SF.Net.Sockets
{
    internal class NetSocket : ISocket
    {
        private readonly NetStream _stream;

        public NetSocket(Socket socket, IPool<Func<IPooled<SocketAsyncEventArgs>>> pool)
        {
            _stream = new NetStream(socket, pool);
        }

        #region ISocket Members

        public Stream Stream
        {
            get { return _stream; }
        }

        public Task Disconnect(CancellationToken cancellation)
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