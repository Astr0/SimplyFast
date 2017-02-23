using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Castle.Zmq;
using SF.Pipes;
using SF.Threading;

namespace SF.Net.Sockets
{
    public class CastleZmqSocket : ISocket, 
        IProducer<IEnumerable<byte[]>>,
        IProducer<byte[]>,
        IConsumer<IReadOnlyList<byte[]>>,
        IConsumer<byte[]>
    {
        private readonly CastleZmqSocketFactory _factory;
        private readonly IZmqSocket _socket;
        private Action _receiveAction;
        private Action _sendAction;

        internal IZmqSocket Socket { get { return _socket; } }

        internal CastleZmqSocket(CastleZmqSocketFactory factory, IZmqSocket socket)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            _factory = factory;
            _socket = socket;
            _factory.AddSocket(this);
        }

        internal void ReceiveReady()
        {
            if (_receiveAction == null) 
                return;
            var act = _receiveAction;
            _receiveAction = null;
            act();
        }

        internal void SendReady()
        {
            if (_sendAction == null) 
                return;
            var act = _sendAction;
            _sendAction = null;
            act();
        }

        private void EnqueueReceive(Action action)
        {
            Debug.Assert(_receiveAction == null, "No multithreading here!");
            _receiveAction = action;
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

        private void EnqueueSend(Action action)
        {
            Debug.Assert(_sendAction == null, "No multithreading here!");
            _sendAction = action;
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
            _factory.RemoveSocket(this);
            _socket.Dispose();
        }

        Stream ISocket.Stream
        {
            get { throw new NotSupportedException("Not supported yet"); }
        }

        public Task Disconnect(CancellationToken cancellation = new CancellationToken())
        {
            Dispose();
            return TaskEx.Completed;
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
                    _socket.Send(data, hasMore);
                }
            }
        }

        private IReadOnlyList<byte[]> Receive()
        {
            var res = new List<byte[]>();
            var hasMore = true;
            while (hasMore)
            {
                res.Add(_socket.Recv());
                hasMore = _socket.HasMoreToRecv();
            }

            return res;
        }

        private void SendOne(byte[] data)
        {
            _socket.Send(data);
        }

        private byte[] ReceiveOne()
        {
            return _socket.Recv();
        }

        public void Connect(string address)
        {
            _socket.Connect(address);
        }

        public void Bind(string address)
        {
            _socket.Bind(address);
        }
    }
}