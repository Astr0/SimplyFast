using System;
using System.Collections.Generic;
using SimplyFast.Cache;

namespace SimplyFast.Pool.Internal
{
    internal class StackPool<T, TParam> : IPool<T, TParam>
    {
        private readonly ReturnToPoll<T> _done;
        private readonly InitPooled<T, TParam> _init;
        private readonly Stack<T> _storage = new Stack<T>();

        public StackPool(InitPooled<T, TParam> init, ReturnToPoll<T> done)
        {
            _init = init ?? throw new ArgumentNullException(nameof(init));
            _done = done;
        }

        public Pooled<T> Get(TParam param = default)
        {
            var item = _storage.Count != 0 ? _storage.Pop() : default;
            return new Pooled<T>(this, _init(item, param));
        }

        public CacheStat CacheStat => new CacheStat(_storage.Count);

        public void Return(T item)
        {
            _done?.Invoke(item);
            _storage.Push(item);
        }
    }
}