using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SF.Comparers;

namespace SF.Tests.Comparers
{
    [TestFixture]
    public class CollectionEqualityComparerTests
    {
        private static IEqualityComparer<T[]> GetCollectionComparer<T>()
        {
            return EqualityComparerEx.Collection<T[], T>();
        }

        [Test]
        public void CompareInts()
        {
            var array1 = new[] { 1, 2, 3 };
            var array2 = new[] { 1, 2, 3 };
            var array3 = new[] { 1, 3, 3 };
            var array4 = new[] { 1, 2, 3, 4 };
            var comparer = GetCollectionComparer<int>();
            var hashCode1 = comparer.GetHashCode(array1);
            var hashCode2 = comparer.GetHashCode(array2);
            var hashCode3 = comparer.GetHashCode(array3);
            var hashCode4 = comparer.GetHashCode(array4);
            Assert.AreEqual(hashCode1, hashCode2);
            Assert.AreNotEqual(hashCode1, hashCode3);
            Assert.AreNotEqual(hashCode1, hashCode4);
            Assert.IsTrue(comparer.Equals(array1, array1));
            Assert.IsTrue(comparer.Equals(array1, array2));
            Assert.IsFalse(comparer.Equals(array1, array3));
            Assert.IsFalse(comparer.Equals(array1, array4));
        }

        [Test]
        public void CompareIntsUsingSignComparer()
        {
            var array1 = new[] { 1, 2, 3 };
            var array2 = new[] { 2, 3, 5 };
            var array3 = new[] { 1, 3, -3 };
            var array4 = new[] { 1, 2, 3, 4 };
            var comparer = EqualityComparerEx.Collection<int[], int>(EqualityComparerEx.Func<int>((x, y) => Math.Sign(x) == Math.Sign(y), Math.Sign));
            var hashCode1 = comparer.GetHashCode(array1);
            var hashCode2 = comparer.GetHashCode(array2);
            var hashCode3 = comparer.GetHashCode(array3);
            var hashCode4 = comparer.GetHashCode(array4);
            Assert.AreEqual(hashCode1, hashCode2);
            Assert.AreNotEqual(hashCode1, hashCode3);
            Assert.AreNotEqual(hashCode1, hashCode4);
            Assert.IsTrue(comparer.Equals(array1, array1));
            Assert.IsTrue(comparer.Equals(array1, array2));
            Assert.IsFalse(comparer.Equals(array1, array3));
            Assert.IsFalse(comparer.Equals(array1, array4));
        }

        [Test]
        public void CompareNonGenericInts()
        {
            object array1 = new[] { 1, 2, 3 };
            object array2 = new[] { 1, 2, 3 };
            object array3 = new[] { 1, 3, 3 };
            object array4 = new[] { 1, 2, 3, 4 };
            var comparer = (IEqualityComparer)GetCollectionComparer<int>();
            var hashCode1 = comparer.GetHashCode(array1);
            var hashCode2 = comparer.GetHashCode(array2);
            var hashCode3 = comparer.GetHashCode(array3);
            var hashCode4 = comparer.GetHashCode(array4);
            Assert.AreEqual(hashCode1, hashCode2);
            Assert.AreNotEqual(hashCode1, hashCode3);
            Assert.AreNotEqual(hashCode1, hashCode4);
            Assert.IsTrue(comparer.Equals(array1, array1));
            Assert.IsTrue(comparer.Equals(array1, array2));
            Assert.IsFalse(comparer.Equals(array1, array3));
            Assert.IsFalse(comparer.Equals(array1, array4));
        }

        [Test]
        public void CompareNonGenericWrong()
        {
            object array = new[] { 1, 2, 3 };
            object array2 = new[] { "1", "2", "3" };
            object list = new List<int> { 1, 2, 3 };
            object str = "123";
            var comparer = (IEqualityComparer)GetCollectionComparer<int>();
            var hashArray = comparer.GetHashCode(array);
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => comparer.GetHashCode(array2));
            Assert.Throws<ArgumentException>(() => comparer.GetHashCode(list));
            Assert.Throws<ArgumentException>(() => comparer.GetHashCode(str));
            Assert.AreNotEqual(0, hashArray);
            Assert.Throws<ArgumentException>(() => comparer.Equals(array, array2));
            Assert.Throws<ArgumentException>(() => comparer.Equals(array, list));
            Assert.Throws<ArgumentException>(() => comparer.Equals(array, str));
            Assert.IsTrue(comparer.Equals(array, array));
            Assert.IsTrue(comparer.Equals(array, (array as ICloneable).Clone()));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void CompareNulls()
        {
            var comparer = GetCollectionComparer<int>();
            Assert.IsTrue(comparer.Equals(null, null));
            Assert.AreEqual(0, comparer.GetHashCode(null));
            Assert.IsFalse(comparer.Equals(new int[1], null));
            Assert.IsFalse(comparer.Equals(null, new int[0]));
        }

    }
}