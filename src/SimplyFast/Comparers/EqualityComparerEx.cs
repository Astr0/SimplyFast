using System;
using System.Collections.Generic;

namespace SimplyFast.Comparers
{
    /// <summary>
    ///     Facade for Comparers
    /// </summary>
    public static class EqualityComparerEx
    {
        /// <summary>
        /// Array equlity comparer using EqualityComparer.Default for elements
        /// </summary>
        public static EqualityComparer<T[]> Array<T>()
        {
            return ArrayEqualityComparer<T>.Instance;
        }

        /// <summary>
        /// Array equlity comparer using elementComparer
        /// </summary>
        public static EqualityComparer<T[]> Array<T>(IEqualityComparer<T> elementComparer)
        {
            return new ArrayEqualityComparer<T>(elementComparer);
        }

        /// <summary>
        /// Collection equlity comparer using EqualityComparer.Default for elements
        /// </summary>
        public static EqualityComparer<TCollection> Collection<TCollection, TItem>() where TCollection : class, IReadOnlyCollection<TItem>
        {
            return CollectionComparer<TCollection, TItem>.Instance;
        }

        /// <summary>
        /// Collection equlity comparer using elementComparer
        /// </summary>
        public static EqualityComparer<TCollection> Collection<TCollection, TItem>(IEqualityComparer<TItem> elementComparer) where TCollection : class, IReadOnlyCollection<TItem>
        {
            return new CollectionComparer<TCollection, TItem>(elementComparer);
        }

        /// <summary>
        /// T equality comparer using TK key
        /// </summary>
        public static EqualityComparer<T> Key<T, TK>(Func<T, TK> keySelector, IEqualityComparer<TK> keyComparer = null)
        {
            return new KeyEqualityComparer<T, TK>(keySelector, keyComparer);
        }

        /// <summary>
        /// T equality comparer from comparison delegate
        /// </summary>
        public static EqualityComparer<T> Func<T>(Func<T, T, bool> comparer, Func<T, int> hash = null)
        {
            return new FuncEqualityComparer<T>(comparer, hash);
        }
    }
}