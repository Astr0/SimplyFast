using System;
using System.Collections.Generic;
using System.Threading;

namespace SimplyFast.Disposables
{
    public static partial class DisposableEx
    {
        private class CollectionRemove<T> : IDisposable
        {
            private ICollection<T> _collection;
            private T _item;

            public CollectionRemove(ICollection<T> collection, T item)
            {
                _collection = collection;
                _item = item;
            }

            public void Dispose()
            {
                var col = Interlocked.Exchange(ref _collection, null);
                if (col == null)
                    return;
                col.Remove(_item);
                _item = default(T);
            }
        }
    }
}