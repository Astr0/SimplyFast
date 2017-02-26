using System;
using System.Collections;
using Xunit;
using SimplyFast.Comparers;

namespace SimplyFast.Tests.Comparers
{
    
    public class KeyEqualityComparerTests
    {
        [Fact]
        public void CompareInts()
        {
            var comparer = EqualityComparerEx.Key((int a) => a);
            Assert.True(comparer.Equals(1, 1));
            Assert.False(comparer.Equals(1, 2));
            Assert.Equal(comparer.GetHashCode(1), comparer.GetHashCode(1));
            Assert.NotEqual(comparer.GetHashCode(1), comparer.GetHashCode(2));
        }

        [Fact]
        public void CompareTestClassByFirstProp()
        {
            var o1 = new ComparersTestClass {A = "test1", B = 1};
            var oe1 = new ComparersTestClass {A = "test1", B = 2};
            var o2 = new ComparersTestClass {A = "test2", B = 2};
            var comparer = EqualityComparerEx.Key((ComparersTestClass a) => a.A);
            Assert.True(comparer.Equals(o1, o1));
            Assert.True(comparer.Equals(o1, oe1));
            Assert.False(comparer.Equals(o1, o2));
            Assert.Equal(comparer.GetHashCode(o1), comparer.GetHashCode(o1));
            Assert.Equal(comparer.GetHashCode(o1), comparer.GetHashCode(oe1));
            Assert.NotEqual(comparer.GetHashCode(o1), comparer.GetHashCode(o2));
        }

        [Fact]
        public void CompareTestClassBySecondProp()
        {
            var o1 = new ComparersTestClass {A = "test1", B = 1};
            var oe1 = new ComparersTestClass {A = "test1", B = 1};
            var o2 = new ComparersTestClass {A = "test1", B = 2};
            var comparer = EqualityComparerEx.Key((ComparersTestClass a) => a.B);
            Assert.True(comparer.Equals(o1, o1));
            Assert.True(comparer.Equals(o1, oe1));
            Assert.False(comparer.Equals(o1, o2));
            Assert.Equal(comparer.GetHashCode(o1), comparer.GetHashCode(o1));
            Assert.Equal(comparer.GetHashCode(o1), comparer.GetHashCode(oe1));
            Assert.NotEqual(comparer.GetHashCode(o1), comparer.GetHashCode(o2));
        }

        [Fact]
        public void NonGenericCompareInts()
        {
            var comparer = (IEqualityComparer)EqualityComparerEx.Key((int a) => a);
            object one = 1;
            object two = 2;
            Assert.True(comparer.Equals(one, 1));
            Assert.False(comparer.Equals(one, two));
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => comparer.Equals(one, "1"));
            Assert.Equal(comparer.GetHashCode(one), comparer.GetHashCode(1));
            Assert.NotEqual(comparer.GetHashCode(one), comparer.GetHashCode(two));
            Assert.Throws<ArgumentException>(() => comparer.GetHashCode("1"));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}