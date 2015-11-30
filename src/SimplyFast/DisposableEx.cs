using System;
using System.Collections.Generic;
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

        public static IDisposable Remove<T>(ICollection<T> collection, T item)
        {
            return new CollectionRemove<T>(collection, item);
        }

        public static IDisposable UseContext(this IDisposable disposable, SynchronizationContext context)
        {
            return new ContextDisposable(disposable, context);
        }


        public static IDisposable DisposeOnFinalize(this IDisposable disposable)
        {
            return new FinalizeDisposable(disposable);
        }

        private class ContextDisposable : IDisposable
        {
            private readonly IDisposable _disposable;
            private readonly SynchronizationContext _context;


            public ContextDisposable(IDisposable disposable, SynchronizationContext context)
            {
                _disposable = disposable;
                _context = context;
            }

            public void Dispose()
            {
                _context.Send(x => ((IDisposable)x).Dispose(), _disposable);
            }
        }

        private class CollectionRemove<T> : IDisposable
        {
            private readonly ICollection<T> _collection;
            private readonly T _item;

            public CollectionRemove(ICollection<T> collection, T item)
            {
                _collection = collection;
                _item = item;
            }

            public void Dispose()
            {
                _collection.Remove(_item);
            }
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