using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SF.Pool;

namespace SF.Net.Sockets
{
    public class NetFactory
    {
        private readonly IPool<SocketAsyncEventArgs> _pool;

        public NetFactory(IPool<SocketAsyncEventArgs> pool = null)
        {
            _pool = pool ?? new BasicPool<SocketAsyncEventArgs>();
        }

        public NetServer Listen(EndPoint endPoint, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.IP, int backlog = 10)
        {
            var socket = new Socket(endPoint.AddressFamily, socketType, protocolType);
            socket.Bind(endPoint);
            socket.Listen(backlog);
            return new NetServer(socket, _pool);
        }

        public Task<NetSocket> Connect(EndPoint endPoint, CancellationToken cancellation, SocketType socketType = SocketType.Stream,
            ProtocolType protocolType = ProtocolType.IP)
        {
            cancellation.ThrowIfCancellationRequested();
            var e = _pool.Get();
            var connectToken = new ConnectToken(cancellation, ConnectCancelled, e);
            e.RemoteEndPoint = endPoint;
            e.UserToken = connectToken;
            e.Completed += ConnectCompleted;
            try
            {
                if (!Socket.ConnectAsync(socketType, protocolType, e))
                    ConnectCompleted(null, e);
            }
            catch (Exception ex)
            {
                ClearConnect(e);
                connectToken.TrySetException(ex);
            }
            return connectToken.Task;
        }

        private void ClearConnect(SocketAsyncEventArgs e)
        {
            e.RemoteEndPoint = null;
            e.UserToken = null;
            e.Completed -= ConnectCompleted;
            _pool.Return(e);
        }

        private void ConnectCancelled(ConnectToken token, SocketAsyncEventArgs e)
        {
            if (token.IsCompleted)
                return;
            Socket.CancelConnectAsync(e);
            if (token.IsCompleted)
                return;
            ((ConnectToken) e.UserToken).TrySetCanceled();
            ClearConnect(e);
        }

        private void ConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            var token = ((ConnectToken) e.UserToken);
            if (token == null)
                return;
            if (e.SocketError == SocketError.Success)
                token.TrySetResult(new NetSocket(e.ConnectSocket, _pool));
            else
                token.TrySetException(new SocketException((int) e.SocketError));
            ClearConnect(e);
        }

        public Task<NetSocket> Connect(EndPoint endPoint, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.IP)
        {
            return Connect(endPoint, CancellationToken.None, socketType, protocolType);
        }

        #region Nested type: ConnectToken

        private class ConnectToken : IDisposable
        {
            private readonly TaskCompletionSource<NetSocket> _tcs;
            private CancellationTokenRegistration _cancellationTokenRegistration;

            public ConnectToken(CancellationToken cancellation, Action<ConnectToken, SocketAsyncEventArgs> cancel, SocketAsyncEventArgs e)
            {
                _tcs = new TaskCompletionSource<NetSocket>();
                if (cancellation.CanBeCanceled)
                {
                    _cancellationTokenRegistration = cancellation.Register(() => cancel(this, e));
                }
            }

            public Task<NetSocket> Task
            {
                get { return _tcs.Task; }
            }

            public bool IsCompleted
            {
                get { return Task.IsCompleted; }
            }

            #region IDisposable Members

            public void Dispose()
            {
                _cancellationTokenRegistration.Dispose();
            }

            #endregion

            public void TrySetCanceled()
            {
                _tcs.TrySetCanceled();
                Dispose();
            }

            public void TrySetException(Exception ex)
            {
                _tcs.TrySetException(ex);
                Dispose();
            }

            public void TrySetResult(NetSocket socket)
            {
                _tcs.TrySetResult(socket);
                Dispose();
            }
        }

        #endregion
    }
}