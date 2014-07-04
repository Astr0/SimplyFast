using System.Collections.Generic;

namespace SF
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
    }
}