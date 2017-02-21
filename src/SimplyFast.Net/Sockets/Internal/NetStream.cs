using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SF.Pool;
using SF.Threading;

namespace SF.Net.Sockets
{
    internal class NetStream : NetworkStream
    {
        private readonly IPool<Func<IPooled<SocketAsyncEventArgs>>> _pool;

        public NetStream(Socket socket, IPool<Func<IPooled<SocketAsyncEventArgs>>> pool) : base(socket)
        {
            _pool = pool;
        }

        public string SocketName => Socket.LocalEndPoint.ToString();

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return TaskEx.FromCancellation<int>(cancellationToken);

            var token = new PooledToken<int>(_pool.Get());
            var e = token.SocketAsyncEventArgs;
            e.SetBuffer(buffer, offset, count);
            e.Completed += ReadCompleted;
            //_socket.Available
            try
            {
                if (!Socket.ReceiveAsync(e))
                    ReadCompleted(Socket, e);
            }
            catch (Exception ex)
            {
                ClearRead(e);
                token.TaskCompletionSource.SetException(ex);
            }
            return token.Task;
        }

        private void ClearRead(SocketAsyncEventArgs e)
        {
            using ((IDisposable) e.UserToken)
            {
                e.SetBuffer(null, 0, 0);
                e.Completed -= ReadCompleted;
            }
        }

        private void ReadCompleted(object sender, SocketAsyncEventArgs e)
        {
            var pooledToken = ((PooledToken<int>) e.UserToken);
            if (e.SocketError == SocketError.Success)
                pooledToken.TaskCompletionSource.SetResult(e.BytesTransferred);
            else
                pooledToken.TaskCompletionSource.SetException(new SocketException((int) e.SocketError));
            ClearRead(e);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return TaskEx.FromCancellation<int>(cancellationToken);

            var token = new PooledToken<bool>(_pool.Get());
            var e = token.SocketAsyncEventArgs;
            e.SetBuffer(buffer, offset, count);
            e.Completed += WriteCompleted;
            try
            {
                if (!Socket.SendAsync(e))
                    WriteCompleted(Socket, e);
            }
            catch (Exception ex)
            {
                ClearWrite(e);
                token.TaskCompletionSource.SetException(ex);
            }
            return token.Task;
        }

        private void ClearWrite(SocketAsyncEventArgs e)
        {
            using ((IDisposable) e.UserToken)
            {
                e.SetBuffer(null, 0, 0);
                e.Completed -= WriteCompleted;
            }
        }

        private void WriteCompleted(object sender, SocketAsyncEventArgs e)
        {
            var pooledToken = ((PooledToken<bool>) e.UserToken);
            if (e.SocketError == SocketError.Success)
                pooledToken.TaskCompletionSource.SetResult(true);
            else
                pooledToken.TaskCompletionSource.SetException(new SocketException((int) e.SocketError));
            ClearWrite(e);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return TaskEx.Completed;
        }

        public Task Disconnect()
        {
            var token = new PooledToken<bool>(_pool.Get());
            var e = token.SocketAsyncEventArgs;
            e.Completed += DisconnectCompleted;
            try
            {
                if (!Socket.DisconnectAsync(e))
                    DisconnectCompleted(Socket, e);
            }
            catch (Exception ex)
            {
                ClearDisconnect(e);
                token.TaskCompletionSource.SetException(ex);
            }
            return token.Task;
        }

        private void ClearDisconnect(SocketAsyncEventArgs e)
        {
            using ((IDisposable)e.UserToken)
            {
                e.Completed -= DisconnectCompleted;
            }
        }

        private void DisconnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            var pooledToken = ((PooledToken<bool>) e.UserToken);
            if (e.SocketError == SocketError.Success)
                pooledToken.TaskCompletionSource.SetResult(true);
            else
                pooledToken.TaskCompletionSource.SetException(new SocketException((int) e.SocketError));
            ClearDisconnect(e);
        }
    }
}