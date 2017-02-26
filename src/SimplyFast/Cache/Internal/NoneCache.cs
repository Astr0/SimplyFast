using System;

namespace SimplyFast.Cache.Internal
{
    internal class NoneCache<TKey, TValue> : ICache<TKey, TValue>
    {
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> createValue, out bool added)
        {
            added = true;
            return createValue(key);
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> createValue)
        {
            return createValue(key);
        }

        public void Upsert(TKey key, TValue value)
        {
        }

        public void Clear()
        {
        }
    }
}