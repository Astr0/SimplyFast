using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Policy;

namespace SF.Collections
{
    /// <summary>
    /// Enumerable utils
    /// </summary>
    public static class EnumerableEx
    {
        /// <summary>
        /// Copy enumerable items to an list starting at start index
        /// </summary>
        public static void CopyTo<T>(this IEnumerable<T> data, IList<T> list, int startIndex = 0)
        {
            Debug.Assert(data != null);
            Debug.Assert(list != null);
            Debug.Assert(startIndex >= 0);
            //Debug.Assert(collection.Count + arrayIndex <= array.Length);
            foreach (var obj in data)
            {
                list[startIndex++] = obj;
            }
        }

        /// <summary>
        /// ToList, but ToHashSet
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(enumerable, comparer);
        }
    }
}