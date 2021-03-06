using System;
using System.Collections.Generic;
using SimplyFast.Collections;

namespace SimplyFast.Comparers
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
                throw new ArgumentNullException(nameof(comparer));
            if (hash == null)
            {
                if (TypeHelper<T>.IsReferenceType)
                    hash = a => !ReferenceEquals(a, null) ? a.GetHashCode() : 0;
                else
                    hash = a => a.GetHashCode();
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