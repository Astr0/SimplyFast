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
        public readonly T Instance;

        public Pooled(IPoolBase<T> pool, T instance)
        {
            _pool = pool;
            Instance = instance;
        }

        public void Dispose()
        {
            var pool = Interlocked.Exchange(ref _pool, null);
            pool?.Return(Instance);
        }
    }
}