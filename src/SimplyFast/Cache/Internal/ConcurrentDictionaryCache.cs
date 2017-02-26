#if CONCURRENT
using System;
using System.Collections.Concurrent;
using SimplyFast.Collections;

namespace SimplyFast.Cache.Internal
{
    internal class ConcurrentDictionaryCache<TK, T> : ICache<TK, T>
    {
        private readonly ConcurrentDictionary<TK, T> _cache = new ConcurrentDictionary<TK, T>();

        public bool TryGetValue(TK key, out T value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public T GetOrAdd(TK key, Func<TK, T> createValue, out bool added)
        {
            return _cache.GetOrAdd(key, createValue, out added);
        }

        public T GetOrAdd(TK key, Func<TK, T> createValue)
        {
            return _cache.GetOrAdd(key, createValue);
        }

        public void Upsert(TK key, T value)
        {
            _cache[key] = value;
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
#endif