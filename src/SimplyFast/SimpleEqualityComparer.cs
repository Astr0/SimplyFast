using System.Collections.Generic;

namespace SF
{
    /// <summary>
    /// Facade for Comparers
    /// </summary>
    public static class SimpleEqualityComparer
    {
        public static EqualityComparer<T[]> Array<T>()
        {
            return ArrayEqualityComparer<T>.Instance;
        }

        public static EqualityComparer<T[]> Array<T>(IEqualityComparer<T> elementComparer)
        {
            return new ArrayEqualityComparer<T>(elementComparer);
        }
    }
}