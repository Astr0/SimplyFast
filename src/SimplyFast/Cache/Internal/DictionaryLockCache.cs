using System;
using System.Collections.Generic;
using SimplyFast.Collections;
using SimplyFast.Disposables;

namespace SimplyFast.Cache.Internal
{
    internal class DictionaryLockCache<TK, T>: ICache<TK, T>
    {
        private readonly Dictionary<TK, T> _cache = new Dictionary<TK, T>();

        public bool TryGetValue(TK key, out T value)
        {
            lock (_cache)
                return _cache.TryGetValue(key, out value);
        }

        public T GetOrAdd(TK key, Func<TK, T> createValue, out bool added)
        {
            lock (_cache)
                return _cache.GetOrAdd(key, createValue, out added);
        }

        public T GetOrAdd(TK key, Func<TK, T> createValue)
        {
            lock (_cache)
                return _cache.GetOrAdd(key, createValue);
        }

        public void Upsert(TK key, T value)
        {
            lock (_cache)
                _cache[key] = value;
        }

        public void Clear()
        {
            lock (_cache)
                _cache.Clear();
        }

        public void Dispose()
        {
            lock (_cache)
            {
                DisposableEx.Dispose(_cache.Values);
                _cache.Clear();
            }
        }
    }
}