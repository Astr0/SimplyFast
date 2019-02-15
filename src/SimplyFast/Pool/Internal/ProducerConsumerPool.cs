using System;
using System.Collections.Concurrent;
using SimplyFast.Cache;

namespace SimplyFast.Pool.Internal
{
    internal class ProducerConsumerPool<T, TParam> : IPool<T, TParam>
    {
        private readonly InitPooled<T, TParam> _init;
        private readonly ReturnToPoll<T> _done;
        private readonly IProducerConsumerCollection<T> _storage;

        public ProducerConsumerPool(InitPooled<T, TParam> init, ReturnToPoll<T> done,
            IProducerConsumerCollection<T> storage = null)
        {
            _init = init ?? throw new ArgumentNullException(nameof(init));
            _done = done;
            _storage = storage ?? new ConcurrentBag<T>();
        }

        public void Return(T getter)
        {
            _done?.Invoke(getter);
            _storage.TryAdd(getter);
        }

        Pooled<T> IPool<T, TParam>.Get(TParam param)
        {
            _storage.TryTake(out var item);
            return new Pooled<T>(this, _init(item, param));
        }

        public CacheStat CacheStat => new CacheStat(_storage.Count);
    }
}