using System;
using System.Collections.Generic;

namespace SF.Collections
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
    }
}