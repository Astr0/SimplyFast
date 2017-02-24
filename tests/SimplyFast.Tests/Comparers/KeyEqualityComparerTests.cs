using System;
using System.Collections;
using NUnit.Framework;
using SimplyFast.Comparers;

namespace SimplyFast.Tests.Comparers
{
    [TestFixture]
    public class KeyEqualityComparerTests
    {
        [Test]
        public void CompareInts()
        {
            var comparer = EqualityComparerEx.Key((int a) => a);
            Assert.IsTrue(comparer.Equals(1, 1));
            Assert.IsFalse(comparer.Equals(1, 2));
            Assert.AreEqual(comparer.GetHashCode(1), comparer.GetHashCode(1));
            Assert.AreNotEqual(comparer.GetHashCode(1), comparer.GetHashCode(2));
        }

        [Test]
        public void CompareTestClassByFirstProp()
        {
            var o1 = new ComparersTestClass {A = "test1", B = 1};
            var oe1 = new ComparersTestClass {A = "test1", B = 2};
            var o2 = new ComparersTestClass {A = "test2", B = 2};
            var comparer = EqualityComparerEx.Key((ComparersTestClass a) => a.A);
            Assert.IsTrue(comparer.Equals(o1, o1));
            Assert.IsTrue(comparer.Equals(o1, oe1));
            Assert.IsFalse(comparer.Equals(o1, o2));
            Assert.AreEqual(comparer.GetHashCode(o1), comparer.GetHashCode(o1));
            Assert.AreEqual(comparer.GetHashCode(o1), comparer.GetHashCode(oe1));
            Assert.AreNotEqual(comparer.GetHashCode(o1), comparer.GetHashCode(o2));
        }

        [Test]
        public void CompareTestClassBySecondProp()
        {
            var o1 = new ComparersTestClass {A = "test1", B = 1};
            var oe1 = new ComparersTestClass {A = "test1", B = 1};
            var o2 = new ComparersTestClass {A = "test1", B = 2};
            var comparer = EqualityComparerEx.Key((ComparersTestClass a) => a.B);
            Assert.IsTrue(comparer.Equals(o1, o1));
            Assert.IsTrue(comparer.Equals(o1, oe1));
            Assert.IsFalse(comparer.Equals(o1, o2));
            Assert.AreEqual(comparer.GetHashCode(o1), comparer.GetHashCode(o1));
            Assert.AreEqual(comparer.GetHashCode(o1), comparer.GetHashCode(oe1));
            Assert.AreNotEqual(comparer.GetHashCode(o1), comparer.GetHashCode(o2));
        }

        [Test]
        public void NonGenericCompareInts()
        {
            var comparer = (IEqualityComparer)EqualityComparerEx.Key((int a) => a);
            object one = 1;
            object two = 2;
            Assert.IsTrue(comparer.Equals(one, 1));
            Assert.IsFalse(comparer.Equals(one, two));
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => comparer.Equals(one, "1"));
            Assert.AreEqual(comparer.GetHashCode(one), comparer.GetHashCode(1));
            Assert.AreNotEqual(comparer.GetHashCode(one), comparer.GetHashCode(two));
            Assert.Throws<ArgumentException>(() => comparer.GetHashCode("1"));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}