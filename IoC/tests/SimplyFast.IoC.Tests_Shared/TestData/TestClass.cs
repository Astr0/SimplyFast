using System;
using System.Diagnostics.CodeAnalysis;

namespace SF.Tests.IoC.TestData
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    internal class TestClass : IEquatable<TestClass>
    {
        public char C { get; }
        public long I { get; }
        public string Str { get; }


        public TestClass(char c, long i)
        {
            C = c;
            I = i;
        }

        public TestClass(char c, long i, string str)
        {
            C = c;
            I = i;
            Str = str;
        }

        public bool Equals(TestClass other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return C == other.C && I == other.I && String.Equals(Str, other.Str);
        }

        public override string ToString()
        {
            return $"{nameof(C)}: {C}, {nameof(I)}: {I}, {nameof(Str)}: {Str}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TestClass) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = C.GetHashCode();
                hashCode = (hashCode * 397) ^ I.GetHashCode();
                hashCode = (hashCode * 397) ^ (Str?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}