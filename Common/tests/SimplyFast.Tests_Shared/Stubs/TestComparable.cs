using System;

namespace SF.Tests.Stubs
{
    internal class TestComparable : IComparable<TestComparable>, IEquatable<TestComparable>
    {
        private readonly int _a;

        public TestComparable(int a)
        {
            _a = a;
        }

        #region IComparable<TestComparable> Members

        public int CompareTo(TestComparable other)
        {
            return _a.CompareTo(other._a);
        }

        #endregion

        #region IEquatable<TestComparable> Members

        public bool Equals(TestComparable other)
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
            return obj.GetType() == typeof(TestComparable) && Equals((TestComparable)obj);
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