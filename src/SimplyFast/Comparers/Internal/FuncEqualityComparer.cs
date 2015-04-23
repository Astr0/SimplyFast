using System;
using System.Collections.Generic;

namespace SF
{
    /// <summary>
    ///     Equality comparer that uses Func for comparison and Func for get hash code. Allows functional comparers
    /// </summary>
    /// <typeparam name="T">Comparable type</typeparam>
    internal class FuncEqualityComparer<T> : EqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparer;
        private readonly Func<T, int> _hash;

        public FuncEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hash = null)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");
            if (hash == null)
            {
                if (typeof (T).IsValueType)
                    hash = a => a.GetHashCode();
                else
                    hash = a => !ReferenceEquals(a, null) ? a.GetHashCode() : 0;
            }
            _comparer = comparer;
            _hash = hash;
        }

        #region IEqualityComparer<T> Members

        public override bool Equals(T x, T y)
        {
            return _comparer(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return _hash(obj);
        }

        #endregion
    }
}