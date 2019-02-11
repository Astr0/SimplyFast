using System;

namespace SimplyFast.Tests.Stubs
{
    internal class SomeComparable : IComparable<SomeComparable>, IEquatable<SomeComparable>
    {
        private readonly int _a;

        public SomeComparable(int a)
        {
            _a = a;
        }

        #region IComparable<SomeComparable> Members

        public int CompareTo(SomeComparable other)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return _a.CompareTo(other._a);
        }

        #endregion

        #region IEquatable<SomeComparable> Members

        public bool Equals(SomeComparable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._a == _a;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(SomeComparable) && Equals((SomeComparable)obj);
        }

        public override int GetHashCode()
        {
            return _a;
        }

        public override string ToString()
        {
            return _a.ToString();
        }
    }
}