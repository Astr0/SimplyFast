using System;

namespace SimplyFast.Data.Spaces.Interface
{
    public struct TupleType : IEquatable<TupleType>
    {
        public readonly ushort Id;

        public TupleType(ushort id)
        {
            Id = id;
        }

        public bool Equals(TupleType other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TupleType && Equals((TupleType) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}