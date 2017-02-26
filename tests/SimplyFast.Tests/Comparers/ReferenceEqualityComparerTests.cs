using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SimplyFast.Comparers;

namespace SimplyFast.Tests.Comparers
{
    [TestFixture]
    public class ReferenceEqualityComparerTests
    {
        [Test]
        public void ComparerWorksByReference()
        {
            var comparer = (IEqualityComparer<string>)EqualityComparerEx.Reference();
            var str1 = 5.ToString();
            var str2 = 5.ToString();
            Assert.IsFalse(comparer.Equals(str1, str2));
            Assert.AreNotEqual(comparer.GetHashCode(str1), comparer.GetHashCode(str2));
            var str3 = str1;
            Assert.IsTrue(comparer.Equals(str1, str3));
            Assert.AreEqual(comparer.GetHashCode(str1), comparer.GetHashCode(str3));
            Assert.IsTrue(comparer.Equals(null, null));
            Assert.IsFalse(comparer.Equals(str1, null));
            Assert.IsFalse(comparer.Equals(null, str1));
            Assert.AreEqual(comparer.GetHashCode(null), comparer.GetHashCode(null));
        }

        [Test]
        public void ComparerWorksByReferenceNonGeneric()
        {
            var comparer = (IEqualityComparer)EqualityComparerEx.Reference();
            var str1 = 5.ToString();
            var str2 = 5.ToString();
            Assert.IsFalse(comparer.Equals(str1, str2));
            Assert.AreNotEqual(comparer.GetHashCode(str1), comparer.GetHashCode(str2));
            var str3 = str1;
            Assert.IsTrue(comparer.Equals(str1, str3));
            Assert.AreEqual(comparer.GetHashCode(str1), comparer.GetHashCode(str3));
            Assert.IsTrue(comparer.Equals(null, null));
            Assert.IsFalse(comparer.Equals(str1, null));
            Assert.IsFalse(comparer.Equals(null, str1));
            Assert.AreEqual(comparer.GetHashCode(null), comparer.GetHashCode(null));
        }
    }
}