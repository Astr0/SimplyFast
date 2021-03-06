﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Castle.Zmq;

namespace SF.Net.Sockets
{
    public class CastleZmqSocketFactory : IDisposable
    {
        private readonly IZmqContext _context;
        //private readonly Poller _poller;
        private readonly ConcurrentQueue<Action> _pollerTasks = new ConcurrentQueue<Action>();
        private readonly Dictionary<IZmqSocket, CastleZmqSocket> _sockets = new Dictionary<IZmqSocket, CastleZmqSocket>(); 
        //private readonly NetMQTimer _timer;

        public CastleZmqSocketFactory()
            : this(new Context())
        {
        }

        public CastleZmqSocketFactory(IZmqContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        #region Create Sockets

        public CastleZmqSocket CreateDealer()
        {
            return Wrap(_context.CreateSocket(SocketType.Dealer));
        }

        public CastleZmqSocket CreateRouter()
        {
            return Wrap(_context.CreateSocket(SocketType.Router));
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public CastleZmqSubSocket CreateSubSocket()
        {
            return new CastleZmqSubSocket(this, _context.CreateSocket(SocketType.Sub));
        }

        #endregion

        private void ExecuteOnPoller(Action action)
        {
            _pollerTasks.Enqueue(action);
        }

        internal void AddSocket(CastleZmqSocket socket)
        {
            ExecuteOnPoller(() => _sockets[socket.Socket] = socket);
        }

        internal void RemoveSocket(CastleZmqSocket socket)
        {
            ExecuteOnPoller(() => _sockets.Remove(socket.Socket));
        }

        private Polling _polling;

        public void Poll()
        {
            ExecutePollerTasks();
            _polling?.PollNow();
        }

        private void ExecutePollerTasks()
        {
            Action action;
            var doneSomething = false;
            while (_pollerTasks.TryDequeue(out action))
            {
                action();
                doneSomething = true;
            }
            if (!doneSomething) 
                return;
            if (_sockets.Count > 0)
            {
                // rebuild poller
                _polling = new Polling(PollingEvents.SendReady | PollingEvents.RecvReady, _sockets.Keys.ToArray());
                _polling.SendReady += s =>
                {
                    //Console.WriteLine("Send ready");
                    CastleZmqSocket socket;
                    if (_sockets.TryGetValue(s, out socket))
                        socket.SendReady();
                };

                _polling.RecvReady += s =>
                {
                    //Console.WriteLine("Recv ready");
                    CastleZmqSocket socket;
                    if (_sockets.TryGetValue(s, out socket))
                        socket.ReceiveReady();
                };
            }
            else
            {
                _polling = null;
            }
        }

        public CastleZmqSocket Wrap(IZmqSocket socket)
        {
            return new CastleZmqSocket(this, socket);
        }
    }
}