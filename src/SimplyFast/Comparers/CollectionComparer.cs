using System.Collections.Generic;
using System.Linq;

namespace SimplyFast.Comparers
{
    internal class CollectionComparer<TCollection, TItem> : EqualityComparer<TCollection>
        where TCollection: class, IReadOnlyCollection<TItem>
    {
        public static readonly EqualityComparer<TCollection> Instance = new CollectionComparer<TCollection, TItem>();

        private readonly IEqualityComparer<TItem> _elementComparer;

        public CollectionComparer(IEqualityComparer<TItem> elementComparer = null)
        {
            _elementComparer = elementComparer ?? EqualityComparer<TItem>.Default;
        }

        public override bool Equals(TCollection x, TCollection y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x == null)
                return false;
            if (y == null)
                return false;
            return x.Count == y.Count && x.SequenceEqual(y, _elementComparer);
        }

        public override int GetHashCode(TCollection obj)
        {
            if (obj == null)
                return 0;
            unchecked
            {
                return obj.Aggregate(0, (current, item) => (current*397) ^ _elementComparer.GetHashCode(item));
            }
        }
    }
}