using SimplyFast.Cache.Internal;

namespace SimplyFast.Cache
{
    public static class CacheEx
    {
        public static ICache<TK, T> None<TK, T>()
        {
            return new NoneCache<TK, T>();
        }

        public static ICache<TK, T> ThreadSafe<TK, T>()
        {
            return Concurrent<TK, T>();
        }

        public static ICache<TK, T> ThreadSafeLocking<TK, T>()
        {
            return new DictionaryLockCache<TK, T>();
        }

        public static ICache<TK, T> ThreadUnsafe<TK, T>()
        {
            return new DictionaryCache<TK, T>();
        }

        /// <summary>
        /// Basic pooling using underlying producer consumer collection
        /// </summary>
        public static ICache<TK, T> Concurrent<TK, T>()
        {
            return new ConcurrentDictionaryCache<TK, T>();
        }

        public static bool TryAdd<TK, T>(this ICache<TK, T> cache, TK key, T value)
        {
            cache.GetOrAdd(key, x => value, out bool added);
            return added;
        }
    }
}