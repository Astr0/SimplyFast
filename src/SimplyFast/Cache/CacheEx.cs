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
#if CONCURRENT
            return Concurrent<TK, T>();
#else
            return ThreadSafeLocking<TK, T>();
#endif
        }

        public static ICache<TK, T> ThreadSafeLocking<TK, T>()
        {
            return new DictionaryLockCache<TK, T>();
        }

        public static ICache<TK, T> ThreadUnsafe<TK, T>()
        {
            return new DictionaryCache<TK, T>();
        }

#if CONCURRENT
        /// <summary>
        /// Basic pooling using underlying producer consumer collection
        /// </summary>
        public static ICache<TK, T> Concurrent<TK, T>()
        {
            return new ConcurrentDictionaryCache<TK, T>();
        }
#endif

        public static bool TryAdd<TK, T>(this ICache<TK, T> cache, TK key, T value)
        {
            cache.GetOrAdd(key, x => value, out bool added);
            return added;
        }
    }
}