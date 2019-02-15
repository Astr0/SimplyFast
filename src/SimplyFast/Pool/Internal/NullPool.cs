using System;
using SimplyFast.Cache;

namespace SimplyFast.Pool.Internal
{
    internal class NullPool<T, TParam>: IPool<T, TParam>
    {
        private readonly InitPooled<T, TParam> _init;
        private readonly ReturnToPoll<T> _done;

        public NullPool(InitPooled<T, TParam> init, ReturnToPoll<T> done)
        {
            _init = init ?? throw new ArgumentNullException(nameof(init));
            _done = done;
        }

        public void Return(T item)
        {
            _done?.Invoke(item);
        }

        public Pooled<T> Get(TParam param = default)
        {
            return new Pooled<T>(this, _init(default, param));
        }

        public CacheStat CacheStat => new CacheStat(0);
    }
}