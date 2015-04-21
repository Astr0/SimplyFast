using System;
using System.Threading;

namespace SF
{
    public static class DisposableEx
    {
        public static IDisposable KeepAlive(this IDisposable disposable, object obj)
        {
            return new KeepAliveDisposable(disposable, obj);
        }

        public static IDisposable Null()
        {
            return NullDisposable.Instance;
        }

        public static IDisposable Concat(params IDisposable[] disposables)
        {
            return new Disposables(disposables);
        }

        public static IDisposable Action(Action disposeAction)
        {
            return new DisposableAction(disposeAction);
        }

        public static IDisposable DisposeOnFinalize(this IDisposable disposable)
        {
            return new FinalizeDisposable(disposable);
        }

        private sealed class DisposableAction : IDisposable
        {
            private readonly Action _action;

            public DisposableAction(Action action)
            {
                if (action == null)
                    throw new ArgumentNullException("action");
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }

        private sealed class Disposables : IDisposable
        {
            private readonly IDisposable[] _disposables;

            public Disposables(params IDisposable[] disposables)
            {
                _disposables = disposables;
            }

            #region IDisposable Members

            public void Dispose()
            {
                if (_disposables == null)
                    return;
                foreach (var disposable in _disposables)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    // ReSharper disable EmptyGeneralCatchClause
                    catch
                    // ReSharper restore EmptyGeneralCatchClause
                    {
                    }
                }
            }

            #endregion
        }

        private class FinalizeDisposable : IDisposable
        {
            public FinalizeDisposable(IDisposable underlying)
            {
                _underlying = underlying;
            }

            ~FinalizeDisposable()
            {
                Dispose();
            }

            private readonly IDisposable _underlying;
            private int _disposed;

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 1)
                    return;
                _underlying.Dispose();
            }
        }

        private class KeepAliveDisposable : IDisposable
        {
            private readonly IDisposable _underlying;
            private readonly object _keepAliveObject;

            public KeepAliveDisposable(IDisposable underlying, object keepAliveObject)
            {
                _underlying = underlying;
                _keepAliveObject = keepAliveObject;
            }

            public void Dispose()
            {
                _underlying.Dispose();
            }
        }

        private class NullDisposable : IDisposable
        {
            public static readonly NullDisposable Instance = new NullDisposable();

            private NullDisposable()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}