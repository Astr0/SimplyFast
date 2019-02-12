using System.Collections.Concurrent;
using SimplyFast.Cache;

namespace SimplyFast.Pool.Internal
{
    internal class ProducerConsumerPool<TGetter> : IPool<TGetter>
    {
        private readonly PooledFactory<TGetter> _factory;
        private readonly IProducerConsumerCollection<TGetter> _storage;

        public ProducerConsumerPool(PooledFactory<TGetter> factory,
            IProducerConsumerCollection<TGetter> storage = null)
        {
            _factory = factory;
            _storage = storage ?? new ConcurrentBag<TGetter>();
        }


        public TGetter Get => _storage.TryTake(out TGetter getFromPool) ? getFromPool : _factory(Return);

        private void Return(TGetter getter)
        {
            _storage.TryAdd(getter);
        }

        public CacheStat CacheStat => new CacheStat(_storage.Count);
    }
}