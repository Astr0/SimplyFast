using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.Concurrent;


namespace SimplyFast.Threading.Internal
{
#pragma warning disable 420
    internal sealed class EventLoopImplementation : IDisposable
    {
        /*
         * Exception handling copied from WindowsForms:
         * 1. Send on same thread - throws out of send
         * 2. Post on same thread - throws AppDomain.Unhandled exception, but continues execution o.O
         * 3. Send on different thread - throws out of send on same thread
         * 4. Post on different thread - throws AppDomain.Unhandled exception, but continues execution o.O
         */

        /*
         * Post behavior
         * Post than Send
         * 1. Same thread - send is queued and executed after post
         * 2. Diffrent thread - send is queued and executed after post
         */
        private readonly CountdownEvent _operationsZero = new CountdownEvent(0);
        private readonly ManualResetEvent _queueHasSomething = new ManualResetEvent(false);
        private volatile int _queueSize;
        private volatile bool _running;
        private readonly ThreadLocal<bool> _currentThread;

        private readonly ConcurrentQueue<WorkItem> _queue = new ConcurrentQueue<WorkItem>();

        private void Enqueue(WorkItem wi)
        {
            _queue.Enqueue(wi);
        }

        private bool TryDequeue(out WorkItem wi)
        {
            return _queue.TryDequeue(out wi);
        }
        //private readonly Queue<WorkItem> _queue = new Queue<WorkItem>();

        //private void Enqueue(WorkItem wi)
        //{
        //    lock (_queue)
        //        _queue.Enqueue(wi);
        //}

        //private bool TryDequeue(out WorkItem wi)
        //{
        //    lock (_queue)
        //    {
        //        if (_queue.Count == 0)
        //        {
        //            wi = default(WorkItem);
        //            return false;
        //        }
        //        wi = _queue.Dequeue();
        //        return true;
        //    }
        //}


        public EventLoopImplementation()
        {
            _currentThread = new ThreadLocal<bool>(false)
            {
                Value = true
            };
        }

#region IDisposable Members

        public void Dispose()
        {
            _running = false;
            _operationsZero.Dispose();
            _queueHasSomething.Dispose();
        }

#endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OperationStarted()
        {
            if (!_operationsZero.TryAddCount())
                _operationsZero.Reset(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OperationCompleted()
        {
            _operationsZero.Signal();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Post(SendOrPostCallback action, object state)
        {
            if (!_running)
                throw new InvalidOperationException("Event loop is not running.");
            Enqueue(new WorkItem(action, state));
            // increment after Enqueue, otherwise DoEvents may fail
            if (Interlocked.Increment(ref _queueSize) == 1)
                _queueHasSomething.Set();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Send(SendOrPostCallback action, object state)
        {
            if (!_running)
                throw new InvalidOperationException("Event loop is not running");
            if (_currentThread.IsValueCreated)
            {
                // Run sync if same thread
                DoProcessCurrentQueue();
                // Run with throw out
                action(state);
            }
            else
            {
                using (var wh = new ManualResetEventSlim(false))
                {
                    Exception thrownException = null;
                    // Post to event loop thread
                    Post(s =>
                    {
                        try
                        {
                            action(s);
                        }
                        catch (Exception ex)
                        {
                            thrownException = ex;
                        }
                        finally
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            wh.Set();
                        }
                    }, state);

                    // wait for completion or termination
                    while (!wh.Wait(1))
                    {
                        if (!_running)
                            throw new InvalidOperationException("Event Loop stopped before completing operation");
                    }

                    // Rethrow exception in Send thread if any
                    if (thrownException != null)
                        throw thrownException;
                }
            }
        }

        public event Action<Exception> UnhandledException;

        private void RaiseUnhandledException(Exception exception)
        {
            var handler = UnhandledException;
            if (handler == null)
                return;
            try
            {
                handler(exception);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        private void DoProcessCurrentQueue()
        {
            // We do this only in Event Loop thread, so _queueSize can only be incremented by Post, no decrements ever
            var queueSize = _queueSize;
            for (var count = queueSize; count >= 0; count--)
            {
                WorkItem wi;
                TryDequeue(out wi);
                try
                {
                    wi.Execute();
                }
                catch (Exception ex)
                {
                    RaiseUnhandledException(ex);
                }
            }
            if (Interlocked.Add(ref _queueSize, -queueSize) == 0)
                _queueHasSomething.Reset();
        }

        public void Run(SendOrPostCallback init, object state)
        {
            if (_running)
                throw new InvalidOperationException("Event loop already running");
            _running = true;
            try
            {
                // Process init here
                init(state);
                // Run the loop
                RunEventLoop();
            }
            finally
            {
                _running = false;
            }
        }

        private void RunEventLoop()
        {
            var toWait = new[] { _queueHasSomething, _operationsZero.WaitHandle };
            // we should run while async operations running or we have stuff in queue
            while (true)
            {
                // Process queue
                WorkItem wi;
                var countProcessed = 0;
                while (TryDequeue(out wi))
                {
                    countProcessed++;
                    try
                    {
                        wi.Execute();
                    }
                    catch (Exception ex)
                    {
                        RaiseUnhandledException(ex);
                    }
                }
                if (Interlocked.Add(ref _queueSize, -countProcessed) == 0)
                    _queueHasSomething.Reset();

                // queue processed, terminate if no async operations
                if (_operationsZero.IsSet)
                    break;

                // ok, we have async operations... wait till all of them completes or something is added to queue
                WaitHandle.WaitAny(toWait);
            }
        }

        public void DoEvents()
        {
            if (!_running)
                throw new InvalidOperationException("Event loop is not running.");
            if (!_currentThread.IsValueCreated)
                throw new InvalidOperationException("Event loop is not running on current thread.");
            DoProcessCurrentQueue();
        }

#region Nested type: WorkItem

        private struct WorkItem
        {
            private readonly SendOrPostCallback _callback;
            private readonly object _state;

            public WorkItem(SendOrPostCallback callback, object state)
            {
                _callback = callback;
                _state = state;
            }

            public void Execute()
            {
                _callback(_state);
            }
        }

#endregion
    }
#pragma warning restore 420
}