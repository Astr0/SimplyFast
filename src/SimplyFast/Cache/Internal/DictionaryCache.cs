using System;
using System.Collections.Generic;
using SimplyFast.Collections;

namespace SimplyFast.Cache.Internal
{
    internal class DictionaryCache<TK, T> : ICache<TK, T>
    {
        private readonly Dictionary<TK, T> _cache = new Dictionary<TK, T>();

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

        public void Clear()
        {
            _cache.Clear();
        }
    }
}