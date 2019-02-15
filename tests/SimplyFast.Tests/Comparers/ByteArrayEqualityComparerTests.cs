using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;
using SimplyFast.Comparers;

namespace SimplyFast.Tests.Comparers
{
    public class ByteArrayEqualityComparerTests
    {
        private static IEqualityComparer<byte[]> GetArrayComparer()
        {
            return EqualityComparerEx.Array<byte>();
        }

        [Fact]
        public void Compare()
        {
            var array1 = new byte[] { 1, 2, 3 };
            var array2 = new byte[] { 1, 2, 3 };
            var array3 = new byte[] { 1, 3, 3 };
            var array4 = new byte[] { 1, 2, 3, 4 };
            var comparer = GetArrayComparer();
            var hashCode1 = comparer.GetHashCode(array1);
            var hashCode2 = comparer.GetHashCode(array2);
            var hashCode3 = comparer.GetHashCode(array3);
            var hashCode4 = comparer.GetHashCode(array4);
            Assert.Equal(hashCode1, hashCode2);
            Assert.NotEqual(hashCode1, hashCode3);
            Assert.NotEqual(hashCode1, hashCode4);
            Assert.True(comparer.Equals(array1, array1));
            Assert.True(comparer.Equals(array1, array2));
            Assert.False(comparer.Equals(array1, array3));
            Assert.False(comparer.Equals(array1, array4));
        }

        [Fact]
        public void CompareNonGeneric()
        {
            object array1 = new byte[] { 1, 2, 3 };
            object array2 = new byte[] { 1, 2, 3 };
            object array3 = new byte[] { 1, 3, 3 };
            object array4 = new byte[] { 1, 2, 3, 4 };
            var comparer = (IEqualityComparer)GetArrayComparer();
            var hashCode1 = comparer.GetHashCode(array1);
            var hashCode2 = comparer.GetHashCode(array2);
            var hashCode3 = comparer.GetHashCode(array3);
            var hashCode4 = comparer.GetHashCode(array4);
            Assert.Equal(hashCode1, hashCode2);
            Assert.NotEqual(hashCode1, hashCode3);
            Assert.NotEqual(hashCode1, hashCode4);
            Assert.True(comparer.Equals(array1, array1));
            Assert.True(comparer.Equals(array1, array2));
            Assert.False(comparer.Equals(array1, array3));
            Assert.False(comparer.Equals(array1, array4));
        }

        [Fact]
        public void CompareNonGenericWrong()
        {
            object array = new byte[] { 1, 2, 3 };
            object array2 = new[] { "1", "2", "3" };
            object list = new List<byte> { 1, 2, 3 };
            object str = "123";
            var comparer = (IEqualityComparer)GetArrayComparer();
            var hashArray = comparer.GetHashCode(array);
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => comparer.GetHashCode(array2));
            Assert.Throws<ArgumentException>(() => comparer.GetHashCode(list));
            Assert.Throws<ArgumentException>(() => comparer.GetHashCode(str));
            Assert.NotEqual(0, hashArray);
            Assert.Throws<ArgumentException>(() => comparer.Equals(array, array2));
            Assert.Throws<ArgumentException>(() => comparer.Equals(array, list));
            Assert.Throws<ArgumentException>(() => comparer.Equals(array, str));
            Assert.True(comparer.Equals(array, array));
            Assert.True(comparer.Equals(array, ((Array) array).Clone()));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Fact]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void CompareNulls()
        {
            var comparer = GetArrayComparer();
            Assert.True(comparer.Equals(null, null));
            Assert.Equal(0, comparer.GetHashCode(null));
            Assert.False(comparer.Equals(new byte[1], null));
            Assert.False(comparer.Equals(null, new byte[0]));
        }

        [Fact]
        public void LongerArraysOk()
        {
            var comparer = GetArrayComparer();
            for (var len = 1; len < 15; ++len)
            {
                var arr1 = Enumerable.Range(0, len).Select(x => (byte) x).ToArray();
                var arr2 = Enumerable.Range(0, len).Select(x => (byte) x).ToArray();
                var arr3 = Enumerable.Range(0, len).Select(x => (byte) (x + 1)).ToArray();
                Assert.Equal(comparer.GetHashCode(arr1), comparer.GetHashCode(arr2));
                Assert.True(comparer.Equals(arr1, arr2));
                Assert.NotEqual(comparer.GetHashCode(arr1), comparer.GetHashCode(arr3));
                Assert.False(comparer.Equals(arr1, arr3));
            }
        }

    }
}