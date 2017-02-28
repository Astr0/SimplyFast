using System;

namespace SimplyFast.Cache
{
    public interface ICache<TKey, TValue>: IDisposable
    {
        bool TryGetValue(TKey key, out TValue value);
        TValue GetOrAdd(TKey key, Func<TKey, TValue> createValue, out bool added);
        TValue GetOrAdd(TKey key, Func<TKey, TValue> createValue);
        void Upsert(TKey key, TValue value);
        void Clear();
    }
}