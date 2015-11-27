using System;
using System.Threading;
using System.Threading.Tasks;
using SF.Pipes;
using SF.Threading;

namespace SF.Tests.Pipes
{
    internal class EventConsumer<T> : IConsumer<T>
    {
        // TODO: Do it right!
        enum State
        {
            Nothing,
            Event,
            Take
        }

        private readonly IDisposable _unsubscribe;
        private State _state;
        private readonly object _lock = new object();
        public EventConsumer(Func<Action<T>, IDisposable> subscribe)
        {
            _unsubscribe = subscribe(OnEvent);
            _state = State.Nothing;
        }

        private T _pendingEvent;
        private TaskCompletionSource<T> _pendingTake;

        public void OnEvent(T value)
        {
            lock (_lock)
            {
                switch (_state)
                {
                    case State.Nothing:
                        _pendingEvent = value;
                        _pendingTake = new TaskCompletionSource<T>();
                        _state = State.Event;
                        break;
                    case State.Take:
                        _pendingTake.TrySetResult(value);
                        _state = State.Nothing;
                        break;
                    case State.Event:
                        WaitForEventTaken();
                        goto case State.Nothing;
                }
            }
        }

        private void WaitForEventTaken()
        {
            // TODO
            do
            {
                var t = _pendingTake;
                t.Task.Wait();
            } while (_state == State.Take);

        }

        public Task<T> Take(CancellationToken cancellation = new CancellationToken())
        {
            if (cancellation.IsCancellationRequested)
                return TaskEx.FromCancellation<T>(cancellation);
            lock (_lock)
            {
                switch (_state)
                {
                    case State.Nothing:
                        var tcs = new TaskCompletionSource<T>();
                        if (cancellation.CanBeCanceled)
                            cancellation.Register(() =>
                            {
                                if (tcs == _pendingTake && _state == State.Take)
                                    _state = State.Nothing;
                                tcs.TrySetCanceled();
                            });
                        _pendingTake = tcs;
                        _state = State.Take;
                        return tcs.Task;
                    case State.Take:
                        throw new Exception("can't Take while Take is pending");
                    case State.Event:
                        var pending = _pendingTake;
                        _state = State.Nothing;
                        pending.SetResult(_pendingEvent);
                        return pending.Task;
                    default:
                        throw new Exception("Invalid state");
                }
            }
        }

        public void Dispose()
        {
            _unsubscribe.Dispose();
        }
    }
}