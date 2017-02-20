using System;
using System.Collections.Concurrent;
using System.Threading;
using NetMQ;

namespace SF.Net.Sockets
{
    public class ZmqSocketFactory : IDisposable
    {
        private readonly NetMQContext _context;
        private readonly Poller _poller;
        private readonly ConcurrentQueue<Action> _pollerTasks = new ConcurrentQueue<Action>();
        private readonly NetMQTimer _timer;

        public ZmqSocketFactory() : this(NetMQContext.Create())
        {
        }

        public ZmqSocketFactory(NetMQContext context, Poller poller = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (poller == null)
            {
                _poller = new Poller();
                var thread = new Thread(pol => ((Poller) pol).Start());
                thread.Start(_poller);
            }
            else
            {
                _poller = poller;
            }
            _context = context;
            _timer = new NetMQTimer(0) {Enable = true};
            _timer.Elapsed += OnTimer;
            _poller.AddTimer(_timer);
        }

        public void Dispose()
        {
            _poller.Stop(false);

            _poller.RemoveTimer(_timer);
            _poller.Dispose();
            _context.Dispose();
        }

        #region Create Sockets

        public ZmqSocket CreateDealer()
        {
            return Wrap(_context.CreateDealerSocket());
        }

        public ZmqSocket CreateRouter()
        {
            return Wrap(_context.CreateRouterSocket());
        }

        public ZmqSubSocket CreateSubSocket()
        {
            return new ZmqSubSocket(this, _context.CreateSubscriberSocket());
        }

        #endregion

        private void ExecuteOnPoller(Action action)
        {
            _pollerTasks.Enqueue(action);
        }

        internal void AddSocket(NetMQSocket socket)
        {
            ExecuteOnPoller(() => _poller.AddSocket(socket));
        }

        internal void RemoveSocket(NetMQSocket socket)
        {
            ExecuteOnPoller(() => _poller.RemoveSocket(socket));
        }

        private void OnTimer(object sender, NetMQTimerEventArgs e)
        {
            Action action;
            while (_pollerTasks.TryDequeue(out action))
                action();
        }

        public ZmqSocket Wrap(NetMQSocket socket)
        {
            return new ZmqSocket(this, socket);
        }
    }
}