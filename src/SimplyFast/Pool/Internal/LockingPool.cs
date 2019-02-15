using System;
using SimplyFast.Cache;

namespace SimplyFast.Pool.Internal
{
    internal class LockingPool<T, TParam>: IPool<T, TParam>
    {
        private readonly object _lock = new object();
        private readonly IPool<T, TParam> _source;

        public LockingPool(IPool<T, TParam> source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public Pooled<T> Get(TParam param = default)
        {
            lock (_lock)
            {
                var polled = _source.Get(param);
                return new Pooled<T>(this, polled.Instance);
            }
        }

        public void Return(T item)
        {
            lock (_lock)
                _source.Return(item);
        }

        public CacheStat CacheStat
        {
            get
            {
                lock (_lock)
                    return _source.CacheStat;
            }
        }
    }
}