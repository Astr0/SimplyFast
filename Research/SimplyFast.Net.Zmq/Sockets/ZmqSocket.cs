using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using SF.Pipes;
using SF.Threading;

namespace SF.Net.Sockets
{
    public class ZmqSocket : ISocket,
        IProducer<IEnumerable<byte[]>>,
        IProducer<byte[]>,
        IConsumer<IReadOnlyList<byte[]>>,
        IConsumer<byte[]>
    {
        protected readonly NetMQSocket Socket;
        private readonly ZmqSocketFactory _factory;
        private Action _receiveAction;
        private Action _sendAction;

        internal ZmqSocket(ZmqSocketFactory factory, NetMQSocket socket)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));
            _factory = factory;
            Socket = socket;
            Socket.SendReady += SendReady;
            Socket.ReceiveReady += ReceiveReady;
            _factory.AddSocket(socket);
        }

        public Task<byte[]> Take(CancellationToken cancellation = new CancellationToken())
        {
            var tcs = new TaskCompletionSource<byte[]>();
            if (tcs.UseCancellation(cancellation))
                return tcs.Task;
            EnqueueReceive(() =>
            {
                if (!cancellation.IsCancellationRequested)
                    tcs.TrySetResult(ReceiveOne());
            });
            return tcs.Task;
        }

        Task<IReadOnlyList<byte[]>> IConsumer<IReadOnlyList<byte[]>>.Take(
            CancellationToken cancellation)
        {
            var tcs = new TaskCompletionSource<IReadOnlyList<byte[]>>();
            if (tcs.UseCancellation(cancellation))
                return tcs.Task;
            EnqueueReceive(() =>
            {
                if (!cancellation.IsCancellationRequested)
                    tcs.TrySetResult(Receive());
            });
            return tcs.Task;
        }

        public Task Add(byte[] obj, CancellationToken cancellation = new CancellationToken())
        {
            var tcs = new TaskCompletionSource<bool>();
            if (tcs.UseCancellation(cancellation))
                return tcs.Task;
            EnqueueSend(() =>
            {
                if (cancellation.IsCancellationRequested)
                    return;
                SendOne(obj);
                tcs.TrySetResult(true);
            });
            return tcs.Task;
        }

        Task IProducer<IEnumerable<byte[]>>.Add(IEnumerable<byte[]> obj,
            CancellationToken cancellation)
        {
            var tcs = new TaskCompletionSource<bool>();
            if (tcs.UseCancellation(cancellation))
                return tcs.Task;
            EnqueueSend(() =>
            {
                if (cancellation.IsCancellationRequested)
                    return;
                Send(obj);
                tcs.TrySetResult(true);
            });
            return tcs.Task;
        }

        public void Dispose()
        {
            Socket.ReceiveReady -= ReceiveReady;
            Socket.SendReady -= SendReady;
            _factory.RemoveSocket(Socket);
            Socket.Dispose();
        }

        Stream ISocket.Stream
        {
            get { throw new NotSupportedException("Not supported yet"); }
        }

        public Task Disconnect(CancellationToken cancellation = new CancellationToken())
        {
            Socket.Close();
            return TaskEx.Completed;
        }

        private void ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var act = Interlocked.Exchange(ref _receiveAction, null);
            act?.Invoke();
        }

        private void SendReady(object sender, NetMQSocketEventArgs e)
        {
            var act = Interlocked.Exchange(ref _sendAction, null);
            act?.Invoke();
        }

        private void EnqueueReceive(Action action)
        {
            var res = Interlocked.CompareExchange(ref _receiveAction, action, null);
            if(res != null)
                throw new InvalidOperationException("No multithreading here!");
        }

        private void EnqueueSend(Action action)
        {
            var res = Interlocked.CompareExchange(ref _sendAction, action, null);
            if (res != null)
                throw new InvalidOperationException("No multithreading here!");
        }

        private void Send(IEnumerable<byte[]> message)
        {
            using (var en = message.GetEnumerator())
            {
                var hasMore = en.MoveNext();
                while (hasMore)
                {
                    var data = en.Current;
                    hasMore = en.MoveNext();
                    Socket.Send(data, data.Length, false, hasMore);
                }
            }
        }

        private IReadOnlyList<byte[]> Receive()
        {
            var res = new List<byte[]>();
            var hasMore = true;
            while (hasMore)
                res.Add(Socket.Receive(out hasMore));
            return res;
        }

        private void SendOne(byte[] data)
        {
            Socket.Send(data, data.Length);
        }

        private byte[] ReceiveOne()
        {
            bool hasMore;
            var res = Socket.Receive(out hasMore);
            while (hasMore)
                Socket.Receive(out hasMore);
            return res;
        }

        public void Connect(string address)
        {
            Socket.Connect(address);
        }

        public void Bind(string address)
        {
            Socket.Bind(address);
        }
    }
}