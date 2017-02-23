using System;
using System.Threading;

namespace SF.Disposables
{
    public static partial class DisposableEx
    {
        private class AssignDisposable<T> : IAssignDisposable<T>
            where T : class, IDisposable
        {
            private T _item;

            public AssignDisposable(T item)
            {
                _item = item;
            }

            public AssignDisposable()
            {
            }

            public T Item
            {
                get { return _item; }
                set
                {
                    var d = Interlocked.Exchange(ref _item, value);
                    if (!ReferenceEquals(d, value) && !ReferenceEquals(d, null))
                        d.Dispose();
                }
            }

            public void Dispose()
            {
                Item = null;
            }
        }
    }
}