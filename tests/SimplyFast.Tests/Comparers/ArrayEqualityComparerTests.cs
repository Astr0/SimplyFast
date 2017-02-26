using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using SimplyFast.Comparers;

namespace SimplyFast.Tests.Comparers
{
    
    public class ArrayEqualityComparerTests
    {
        private static IEqualityComparer<T[]> GetArrayComparer<T>()
        {
            return EqualityComparerEx.Array<T>();
        }

        [Fact]
        public void CompareInts()
        {
            var array1 = new[] { 1, 2, 3 };
            var array2 = new[] { 1, 2, 3 };
            var array3 = new[] { 1, 3, 3 };
            var array4 = new[] { 1, 2, 3, 4 };
            var comparer = GetArrayComparer<int>();
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
        public void CompareIntsUsingSignComparer()
        {
            var array1 = new[] { 1, 2, 3 };
            var array2 = new[] { 2, 3, 5 };
            var array3 = new[] { 1, 3, -3 };
            var array4 = new[] { 1, 2, 3, 4 };
            var comparer = EqualityComparerEx.Array(EqualityComparerEx.Func<int>((x, y) => Math.Sign(x) == Math.Sign(y), Math.Sign));
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
        public void CompareNonGenericInts()
        {
            object array1 = new[] { 1, 2, 3 };
            object array2 = new[] { 1, 2, 3 };
            object array3 = new[] { 1, 3, 3 };
            object array4 = new[] { 1, 2, 3, 4 };
            var comparer = (IEqualityComparer)GetArrayComparer<int>();
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
            object array = new[] { 1, 2, 3 };
            object array2 = new[] { "1", "2", "3" };
            object list = new List<int> { 1, 2, 3 };
            object str = "123";
            var comparer = (IEqualityComparer)GetArrayComparer<int>();
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
        public void CompareNulls()
        {
            var comparer = GetArrayComparer<int>();
            Assert.True(comparer.Equals(null, null));
            Assert.Equal(0, comparer.GetHashCode(null));
            Assert.False(comparer.Equals(new int[1], null));
            Assert.False(comparer.Equals(null, new int[0]));
        }

    }
}