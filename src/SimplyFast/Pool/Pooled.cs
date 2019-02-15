using System;
using System.Threading;

namespace SimplyFast.Pool
{
    /// <summary>
    /// Use only with using block! Multiple dispose or copy will not work
    /// </summary>
    public struct Pooled<T>: IDisposable
    {
        private IPoolBase<T> _pool;
        public readonly T Item;

        public Pooled(IPoolBase<T> pool, T item)
        {
            _pool = pool;
            Item = item;
        }

        public void Dispose()
        {
            var pool = Interlocked.Exchange(ref _pool, null);
            pool?.Return(Item);
        }
    }
}