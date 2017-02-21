using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SF.Pool;

namespace SF.Net.Sockets
{
    // TODO: Add Cancellation support
    internal class NetServer : ISocketServer
    {
        private readonly IPool<Func<IPooled<SocketAsyncEventArgs>>> _pool;
        private readonly Socket _socket;

        public NetServer(Socket socket, IPool<Func<IPooled<SocketAsyncEventArgs>>> pool)
        {
            _socket = socket;
            _pool = pool;
        }


        public Task Close(CancellationToken cancellation)
        {
            var token = new PooledToken<bool>(_pool.Get());
            var e = token.SocketAsyncEventArgs;
            e.Completed += CloseCompleted;
            try
            {
                if (!_socket.DisconnectAsync(e))
                    CloseCompleted(_socket, e);
            }
            catch (Exception ex)
            {
                ClearClose(e);
                token.TaskCompletionSource.SetException(ex);
            }
            return token.Task;
        }

        public void Dispose()
        {
            _socket.Dispose();
        }

        public Task<ISocket> Accept(CancellationToken cancellation)
        {
            var token = new PooledToken<ISocket>(_pool.Get());
            var e = token.SocketAsyncEventArgs;
            e.Completed += AcceptCompleted;
            try
            {
                if (!_socket.AcceptAsync(e))
                    AcceptCompleted(_socket, e);
            }
            catch (Exception ex)
            {
                ClearAccept(e);
                token.TaskCompletionSource.SetException(ex);
            }
            return token.Task;
        }

        private void ClearAccept(SocketAsyncEventArgs e)
        {
            using ((IDisposable) e.UserToken)
                e.Completed -= AcceptCompleted;
        }

        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            var token = (PooledToken<ISocket>) e.UserToken;
            if (e.SocketError == SocketError.Success)
                token.TaskCompletionSource.SetResult(new NetSocket(e.AcceptSocket, _pool));
            else
                token.TaskCompletionSource.SetException(new SocketException((int) e.SocketError));
            e.AcceptSocket = null;
            ClearAccept(e);
        }

        private void ClearClose(SocketAsyncEventArgs e)
        {
            using ((IDisposable)e.UserToken)
                e.Completed -= CloseCompleted;
        }

        private void CloseCompleted(object sender, SocketAsyncEventArgs e)
        {
            ((PooledToken<bool>) e.UserToken).TaskCompletionSource.SetResult(true);
            ClearClose(e);
        }

        public override string ToString()
        {
            return _socket.LocalEndPoint.ToString();
        }
    }

    internal class PooledToken<T>: IDisposable
    {
        private readonly IPooled<SocketAsyncEventArgs> _pooled;
        public readonly TaskCompletionSource<T> TaskCompletionSource;
        public Task<T> Task => TaskCompletionSource.Task;
        public SocketAsyncEventArgs SocketAsyncEventArgs => _pooled.Instance;

        public PooledToken(IPooled<SocketAsyncEventArgs> pooled)
        {
            _pooled = pooled;
            TaskCompletionSource = new TaskCompletionSource<T>();
            _pooled.Instance.UserToken = this;
        }
        
        public void Dispose()
        {
            if (_pooled.Instance.UserToken == this)
                _pooled.Instance.UserToken = null;
            _pooled.Dispose();
        }
    }
}