using System;
using System.Collections.Generic;

namespace SimplyFast.Comparers
{
    /// <summary>
    ///     Compares objects by key
    /// </summary>
    /// <typeparam name="T">Object Type</typeparam>
    /// <typeparam name="TK">Object key Type</typeparam>
    internal class KeyEqualityComparer<T, TK> : EqualityComparer<T>
    {
        private readonly IEqualityComparer<TK> _keyComparer;
        private readonly Func<T, TK> _keySelector;

        public KeyEqualityComparer(Func<T, TK> keySelector, IEqualityComparer<TK> keyComparer = null)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            _keySelector = keySelector;
            _keyComparer = keyComparer ?? EqualityComparer<TK>.Default;
        }

        #region IEqualityComparer<T> Members

        public override bool Equals(T x, T y)
        {
            return _keyComparer.Equals(_keySelector(x), _keySelector(y));
        }

        public override int GetHashCode(T obj)
        {
            return _keyComparer.GetHashCode(_keySelector(obj));
        }

        #endregion
    }
}