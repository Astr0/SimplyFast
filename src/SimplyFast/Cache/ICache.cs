using System;

namespace SimplyFast.Cache
{
    public interface ICache<TKey, TValue>
    {
        bool TryGetValue(TKey key, out TValue value);
        TValue GetOrAdd(TKey key, Func<TKey, TValue> createValue, out bool added);
        TValue GetOrAdd(TKey key, Func<TKey, TValue> createValue);
        void Clear();
    }
}