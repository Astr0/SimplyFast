using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SimplyFast.Collections
{
    /// <summary>
    /// Dictionary utilities
    /// </summary>
    public static class DictionaryEx
    {
        /// <summary>
        ///     Returns value for passed key or adds using add delegate
        /// </summary>
        public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TK, TV> add)
        {
            TV v;
            if (dictionary.TryGetValue(key, out v))
                return v;
            v = add(key);
            dictionary[key] = v;
            return v;
        }

        /// <summary>
        ///     Returns value for passed key or adds using add delegate
        /// </summary>
        public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TK, TV> add, out bool added)
        {
            TV v;
            if (dictionary.TryGetValue(key, out v))
            {
                added = false;
                return v;
            }
            v = add(key);
            dictionary[key] = v;
            added = true;
            return v;
        }

        /// <summary>
        ///     Returns value for passed key or adds new TV()
        /// </summary>
        public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key)
            where TV:new()
        {
            TV v;
            if (dictionary.TryGetValue(key, out v))
                return v;
            v = new TV();
            dictionary[key] = v;
            return v;
        }

        /// <summary>
        ///     Returns value for passed key or returns default(TV)
        /// </summary>
        public static TV GetOrDefault<TK, TV>(this IDictionary<TK, TV> dictionary, TK key)
        {
            TV v;
            dictionary.TryGetValue(key, out v);
            return v;
        }

        /// <summary>
        ///     Returns value for passed key or returns defaultValue
        /// </summary>
        public static TV GetOrDefault<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV defaultValue)
        {
            TV v;
            return dictionary.TryGetValue(key, out v) ? v : defaultValue;
        }

        /// <summary>
        /// Returns value for passed key or add's value using builder. 
        /// Returns flag indicating if value was added. 
        /// Builder maybe executed multiple times in concurrent environment.
        /// </summary>
        public static T GetOrAdd<TK, T>(this ConcurrentDictionary<TK, T> dictionary, TK key, Func<TK, T> builder, out bool added)
        {
            T value;

            // try to get existing
            if (dictionary.TryGetValue(key, out value))
            {
                added = false;
                return value;
            }

            // create new value
            var newValue = builder(key);

            // try to add it
            if (dictionary.TryAdd(key, newValue))
            {
                added = true;
                return newValue;
            }

            while (true)
            {
                // oh sh1t
                if (dictionary.TryGetValue(key, out value))
                {
                    added = false;
                    return value;
                }
                if (!dictionary.TryAdd(key, newValue))
                    continue;
                added = true;
                return newValue;
            }
        }
    }
}