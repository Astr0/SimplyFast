using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using SF.Pool;

namespace SF.Network.Sockets
{
    public class NetSocket : ISocket
    {
        private readonly IPool<SocketAsyncEventArgs> _pool;
        private readonly Socket _socket;

        public NetSocket(Socket socket, IPool<SocketAsyncEventArgs> pool)
        {
            _socket = socket;
            _pool = pool;
        }

        #region ISocket Members

        public void Dispose()
        {
            _socket.Dispose();
        }

        public Task Write(byte[] buffer, int offset, int count)
        {
            var tcs = new TaskCompletionSource<bool>();
            var e = _pool.Get();
            e.SetBuffer(buffer, offset, count);
            e.UserToken = tcs;
            e.Completed += WriteCompleted;
            try
            {
                if (!_socket.SendAsync(e))
                    WriteCompleted(_socket, e);
            }
            catch (Exception ex)
            {
                ClearWrite(e);
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        public Task<int> Read(byte[] buffer, int offset, int count)
        {
            var tcs = new TaskCompletionSource<int>();
            var e = _pool.Get();
            e.SetBuffer(buffer, offset, count);
            e.UserToken = tcs;
            e.Completed += ReadCompleted;
            //_socket.Available
            try
            {
                if (!_socket.ReceiveAsync(e))
                    ReadCompleted(_socket, e);
            }
            catch (Exception ex)
            {
                ClearRead(e);
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        public Task Disconnect()
        {
            var tcs = new TaskCompletionSource<bool>();
            var e = _pool.Get();
            e.UserToken = tcs;
            e.Completed += DisconnectCompleted;
            try
            {
                if (!_socket.DisconnectAsync(e))
                    DisconnectCompleted(_socket, e);
            }
            catch (Exception ex)
            {
                ClearDisconnect(e);
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        #endregion

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

        private void ClearDisconnect(SocketAsyncEventArgs e)
        {
            e.UserToken = null;
            e.Completed -= DisconnectCompleted;
            _pool.Return(e);
        }

        private void DisconnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
                ((TaskCompletionSource<bool>) e.UserToken).SetResult(true);
            else
                ((TaskCompletionSource<bool>) e.UserToken).SetException(new SocketException((int) e.SocketError));
            ClearDisconnect(e);
        }

        public override string ToString()
        {
            return _socket.LocalEndPoint.ToString();
        }
    }
}