using System;

namespace SimplyFast.Reflection.Tests.TestData
{
    public class TestClass1 : IEquatable<TestClass1>
    {
        public string F2 = "test";
        private int _f1 = 1;

        private string P0 { get; set; }

        public string P00
        {
            get { return P0; }
            set { P0 = value; }
        }

        [TestMe(Value = 1)]
        public int P1
        {
            get { return _f1; }
            set { _f1 = value; }
        }

        public string P2 { get; set; }

        [TestMe(Value = 3)]
        public string P3 { get; set; }

        [TestMe(Value = 4)]
        public virtual string P4 { get; set; }

        [TestMe(Value = 5)]
        public virtual string P5 { get; set; }

        #region IEquatable<TestClass1> Members

        public bool Equals(TestClass1 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._f1 == _f1 && Equals(other.F2, F2) && Equals(other.P0, P0) && Equals(other.P2, P2) &&
                   Equals(other.P3, P3) && Equals(other.P4, P4) && Equals(other.P5, P5);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof (TestClass1) && Equals((TestClass1) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = _f1;
                result = (result*397) ^ (F2 != null ? F2.GetHashCode() : 0);
                result = (result*397) ^ (P0 != null ? P0.GetHashCode() : 0);
                result = (result*397) ^ (P2 != null ? P2.GetHashCode() : 0);
                result = (result*397) ^ (P3 != null ? P3.GetHashCode() : 0);
                result = (result*397) ^ (P4 != null ? P4.GetHashCode() : 0);
                result = (result*397) ^ (P5 != null ? P5.GetHashCode() : 0);
                return result;
            }
        }

        public static bool operator ==(TestClass1 left, TestClass1 right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TestClass1 left, TestClass1 right)
        {
            return !Equals(left, right);
        }
    }
}