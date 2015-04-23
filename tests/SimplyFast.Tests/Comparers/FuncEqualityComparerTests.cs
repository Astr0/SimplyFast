using System;
using System.Collections;
using NUnit.Framework;
using SimpleFramework.Tests.Common.Comparers;

namespace SF.Tests.Comparers
{
    [TestFixture]
    public class FuncEqualityComparerTests
    {
        [Test]
        public void CompareInts()
        {
            var comparer = EqualityComparerEx.Func((int a, int b) => a == b);
            Assert.IsTrue(comparer.Equals(1, 1));
            Assert.IsFalse(comparer.Equals(1, 2));
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(comparer.GetHashCode(1) == comparer.GetHashCode(1));
            Assert.IsFalse(comparer.GetHashCode(1) == comparer.GetHashCode(2));
        }

        [Test]
        public void CompareTestClassByOneProp()
        {
            var o1 = new ComparersTestClass {A = "test1", B = 1};
            var oe1 = new ComparersTestClass {A = "test1", B = 2};
            var o2 = new ComparersTestClass {A = "test2", B = 2};
            var comparer = EqualityComparerEx.Func<ComparersTestClass>((a, b) => a.A == b.A, a => a.A.GetHashCode());
            Assert.IsTrue(comparer.Equals(o1, o1));
            Assert.IsTrue(comparer.Equals(o1, oe1));
            Assert.IsFalse(comparer.Equals(o1, o2));
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(comparer.GetHashCode(o1) == comparer.GetHashCode(o1));
            Assert.IsTrue(comparer.GetHashCode(o1) == comparer.GetHashCode(oe1));
            Assert.IsFalse(comparer.GetHashCode(o1) == comparer.GetHashCode(o2));
        }

        [Test]
        public void CompareTestClassByTwoProp()
        {
            var o1 = new ComparersTestClass {A = "test1", B = 1};
            var oe1 = new ComparersTestClass {A = "test1", B = 1};
            var o2 = new ComparersTestClass {A = "test1", B = 2};
            var comparer = EqualityComparerEx.Func<ComparersTestClass>((a, b) => a.A == b.A && a.B == b.B,
                                                                   a =>
                                                                   ((a.A != null ? a.A.GetHashCode() : 0)*397) ^
                                                                   a.B);
            Assert.IsTrue(comparer.Equals(o1, o1));
            Assert.IsTrue(comparer.Equals(o1, oe1));
            Assert.IsFalse(comparer.Equals(o1, o2));
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(comparer.GetHashCode(o1) == comparer.GetHashCode(o1));
            Assert.IsTrue(comparer.GetHashCode(o1) == comparer.GetHashCode(oe1));
            Assert.IsFalse(comparer.GetHashCode(o1) == comparer.GetHashCode(o2));
        }

        [Test]
        public void NonGenericCompareInts()
        {
            var comparer = EqualityComparerEx.Func<int>((a, b) => a == b);
            object one = 1;
            object two = 2;
            var comparerObj = comparer as IEqualityComparer;
            Assert.IsTrue(comparerObj.Equals(one, 1));
            Assert.IsFalse(comparerObj.Equals(one, two));
            // ReSharper disable once EqualExpressionComparison
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => comparerObj.Equals(one, "1"));
            Assert.IsTrue(comparerObj.GetHashCode(one) == comparer.GetHashCode(1));
            Assert.IsFalse(comparerObj.GetHashCode(one) == comparerObj.GetHashCode(two));
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => comparerObj.GetHashCode("1"));
        }
    }
}