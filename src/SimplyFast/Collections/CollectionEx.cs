using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SF.Collections
{
    /// <summary>
    /// Collection utils
    /// </summary>
    public static class CollectionEx
    {
        /// <summary>
        /// Copy collection items to an array starting at start index
        /// </summary>
        public static void CopyTo<T>(this IReadOnlyCollection<T> collection, T[] array, int arrayIndex = 0)
        {
            Debug.Assert(collection != null);
            Debug.Assert(array != null);
            Debug.Assert(arrayIndex >= 0);
            Debug.Assert(collection.Count + arrayIndex <= array.Length);
            foreach (var obj in collection)
            {
                array[arrayIndex++] = obj;
            }
        }

        public static TList CopyTo<TList, T>(this IEnumerable<T> data, TList list, int startIndex = 0)
            where TList: IList<T>
        {
            foreach (var obj in data)
            {
                list[startIndex++] = obj;
            }
            return list;
        }
    }
}