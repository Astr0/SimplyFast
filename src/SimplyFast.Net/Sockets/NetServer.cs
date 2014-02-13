using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using SF.Pool;
using SF.Threading;

namespace SF.Net.Sockets
{
    public class NetServer: ISocketServer
    {
        private readonly Socket _socket;
        private readonly IPool<SocketAsyncEventArgs> _pool;

        public NetServer(Socket socket, IPool<SocketAsyncEventArgs> pool)
        {
            _socket = socket;
            _pool = pool;
        }

        public Task<NetSocket> Accept()
        {
            var tcs = new TaskCompletionSource<NetSocket>();
            var e = _pool.Get();
            e.UserToken = tcs;
            e.Completed += AcceptCompleted;
            try
            {
                if (!_socket.AcceptAsync(e))
                    AcceptCompleted(_socket, e);
            }
            catch (Exception ex)
            {
                ClearAccept(e);
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        private void ClearAccept(SocketAsyncEventArgs e)
        {
            e.UserToken = null;
            e.Completed -= AcceptCompleted;
            _pool.Return(e);
        }

        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
                ((TaskCompletionSource<NetSocket>)e.UserToken).SetResult(new NetSocket(e.AcceptSocket, _pool));
            else
                ((TaskCompletionSource<NetSocket>)e.UserToken).SetException(new SocketException((int)e.SocketError));
            e.AcceptSocket = null;
            ClearAccept(e);
        }

        Task<ISocket> ISocketServer.Accept()
        {
            return Accept().CastToBase<NetSocket, ISocket>();
        }
        
        public Task Close()
        {
            var tcs = new TaskCompletionSource<bool>();
            var e = _pool.Get();
            e.UserToken = tcs;
            e.Completed += CloseCompleted;
            try
            {
                if (!_socket.DisconnectAsync(e))
                    CloseCompleted(_socket, e);
            }
            catch(Exception ex)
            {
                ClearClose(e);
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        private void ClearClose(SocketAsyncEventArgs e)
        {
            e.UserToken = null;
            e.Completed -= CloseCompleted;
            _pool.Return(e);
        }

        private void CloseCompleted(object sender, SocketAsyncEventArgs e)
        {
            ((TaskCompletionSource<bool>)e.UserToken).SetResult(true);
            ClearClose(e);
        }

        public void Dispose()
        {
            _socket.Dispose();
        }

        public override string ToString()
        {
            return _socket.LocalEndPoint.ToString();
        }
    }
}