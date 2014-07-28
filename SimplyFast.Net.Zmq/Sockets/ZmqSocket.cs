using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using SF.Collections;
using SF.Pipes;
using SF.Threading;

namespace SF.Net.Sockets
{
    public class ZmqSocket : ISocket, 
        IProducer<IEnumerable<ArraySegment<byte>>>,
        IProducer<ArraySegment<byte>>,
        IConsumer<ArraySegment<byte>[]>,
        IConsumer<ArraySegment<byte>>
    {
        private readonly ZmqSocketFactory _factory;
        private readonly NetMQSocket _socket;
        private Action _receiveAction;
        private Action _sendAction;

        internal ZmqSocket(ZmqSocketFactory factory, NetMQSocket socket)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            _factory = factory;
            _factory = factory;
            _socket = socket;
            _socket.SendReady += SendReady;
            _socket.ReceiveReady += ReceiveReady;
            _factory.AddSocket(socket);
        }

        private void ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            if (_receiveAction == null) 
                return;
            var act = _receiveAction;
            _receiveAction = null;
            act();
        }

        private void SendReady(object sender, NetMQSocketEventArgs e)
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

        public Task<ArraySegment<byte>> Take(CancellationToken cancellation = new CancellationToken())
        {
            var tcs = new TaskCompletionSource<ArraySegment<byte>>();
            if (tcs.UseCancellation(cancellation))
                return tcs.Task;
            EnqueueReceive(() =>
            {
                if (!cancellation.IsCancellationRequested)
                    tcs.TrySetResult(ReceiveOne());
            });
            return tcs.Task;
        }

        Task<ArraySegment<byte>[]> IConsumer<ArraySegment<byte>[]>.Take(
            CancellationToken cancellation)
        {
            var tcs = new TaskCompletionSource<ArraySegment<byte>[]>();
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

        public Task Add(ArraySegment<byte> obj, CancellationToken cancellation = new CancellationToken())
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

        Task IProducer<IEnumerable<ArraySegment<byte>>>.Add(IEnumerable<ArraySegment<byte>> obj,
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
            _socket.ReceiveReady -= ReceiveReady;
            _socket.SendReady -= SendReady;
            _factory.RemoveSocket(_socket);
            _socket.Dispose();
        }

        public Stream Stream
        {
            get { throw new NotSupportedException("Not supported yet"); }
        }

        public Task Disconnect(CancellationToken cancellation = new CancellationToken())
        {
            _socket.Close();
            return TaskEx.Completed;
        }

        private void Send(IEnumerable<ArraySegment<byte>> message)
        {
            using (var en = message.GetEnumerator())
            {
                var hasMore = en.MoveNext();
                while (hasMore)
                {
                    var data = en.Current;
                    hasMore = en.MoveNext();
                    if (data.Offset == 0)
                    {
                        _socket.Send(data.Array, data.Count);
                    }
                    else
                    {
                        var arr = new byte[data.Count];
                        Array.Copy(data.Array, data.Offset, arr, 0, data.Count);
                        _socket.Send(arr, arr.Length);
                    }
                }
            }
        }

        private ArraySegment<byte>[] Receive()
        {
            var msg = _socket.ReceiveMessage();

            return msg.Select(f => new ArraySegment<byte>(f.Buffer, 0, f.MessageSize)).CopyTo(new ArraySegment<byte>[msg.FrameCount]);
        }

        private void SendOne(ArraySegment<byte> data)
        {
            if (data.Offset == 0)
            {
                _socket.Send(data.Array, data.Count);
            }
            else
            {
                var arr = new byte[data.Count];
                Array.Copy(data.Array, data.Offset, arr, 0, data.Count);
                _socket.Send(arr, arr.Length);
            }
        }

        private ArraySegment<byte> ReceiveOne()
        {
            bool hasMore;
            return new ArraySegment<byte>(_socket.Receive(out hasMore));
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