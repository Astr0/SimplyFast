using System.Collections.Generic;

namespace SF
{
    internal class ArrayEqualityComparer<T> : EqualityComparer<T[]>
    {
        public static readonly EqualityComparer<T[]> Instance = new ArrayEqualityComparer<T>();

        private readonly IEqualityComparer<T> _elementComparer;

        public ArrayEqualityComparer(IEqualityComparer<T> elementComparer = null)
        {
            _elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        }

        public override bool Equals(T[] x, T[] y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null))
                return false;
            if (ReferenceEquals(y, null))
                return false;
            if (x.Length != y.Length)
                return false;
            for (var i = 0; i < x.Length; i++)
            {
                if (!_elementComparer.Equals(x[i], y[i]))
                    return false;
            }
            return true;
        }

        public override int GetHashCode(T[] obj)
        {
            if (obj == null)
                return 0;
            unchecked
            {
                var hashCode = 0;
                foreach (var item in obj)
                {
                    hashCode = (hashCode*397) ^ _elementComparer.GetHashCode(item);
                }
                return hashCode;
            }
        }
    }
}