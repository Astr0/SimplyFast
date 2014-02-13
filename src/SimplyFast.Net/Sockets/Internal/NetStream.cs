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
        private readonly IPool<SocketAsyncEventArgs> _pool;

        public NetStream(Socket socket, IPool<SocketAsyncEventArgs> pool) : base(socket)
        {
            _pool = pool;
        }

        public string SocketName { get { return Socket.LocalEndPoint.ToString(); } }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return TaskEx.FromCancellation<int>(cancellationToken);

            var tcs = new TaskCompletionSource<int>();
            var e = _pool.Get();
            e.SetBuffer(buffer, offset, count);
            e.UserToken = tcs;
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
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        private void ClearRead(SocketAsyncEventArgs e)
        {
            e.SetBuffer(null, 0, 0);
            e.UserToken = null;
            e.Completed -= ReadCompleted;
            _pool.Return(e);
        }

        private void ReadCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                ((TaskCompletionSource<int>) e.UserToken).SetResult(e.BytesTransferred);
            }
            else
                ((TaskCompletionSource<int>) e.UserToken).SetException(new SocketException((int) e.SocketError));
            ClearRead(e);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return TaskEx.FromCancellation<int>(cancellationToken);

            var tcs = new TaskCompletionSource<bool>();
            var e = _pool.Get();
            e.SetBuffer(buffer, offset, count);
            e.UserToken = tcs;
            e.Completed += WriteCompleted;
            try
            {
                if (!Socket.SendAsync(e))
                    WriteCompleted(Socket, e);
            }
            catch (Exception ex)
            {
                ClearWrite(e);
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        private void ClearWrite(SocketAsyncEventArgs e)
        {
            e.SetBuffer(null, 0, 0);
            e.UserToken = null;
            e.Completed -= WriteCompleted;
            _pool.Return(e);
        }

        private void WriteCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
                ((TaskCompletionSource<bool>) e.UserToken).SetResult(true);
            else
                ((TaskCompletionSource<bool>) e.UserToken).SetException(new SocketException((int) e.SocketError));
            ClearWrite(e);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return TaskEx.Completed;
        }

        public Task Disconnect()
        {
            var tcs = new TaskCompletionSource<bool>();
            var e = _pool.Get();
            e.UserToken = tcs;
            e.Completed += DisconnectCompleted;
            try
            {
                if (!Socket.DisconnectAsync(e))
                    DisconnectCompleted(Socket, e);
            }
            catch (Exception ex)
            {
                ClearDisconnect(e);
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        private void ClearDisconnect(SocketAsyncEventArgs e)
        {
            e.UserToken = null;
            e.Completed -= DisconnectCompleted;
            _pool.Return(e);
        }

        private void DisconnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
                ((TaskCompletionSource<bool>)e.UserToken).SetResult(true);
            else
                ((TaskCompletionSource<bool>)e.UserToken).SetException(new SocketException((int)e.SocketError));
            ClearDisconnect(e);
        }
    }
}