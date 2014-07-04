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
    }
}