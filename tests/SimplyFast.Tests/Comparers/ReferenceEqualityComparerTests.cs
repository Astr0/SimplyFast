using System.Collections;
using System.Collections.Generic;
using Xunit;
using SimplyFast.Comparers;

namespace SimplyFast.Tests.Comparers
{
    
    public class ReferenceEqualityComparerTests
    {
        [Fact]
        public void ComparerWorksByReference()
        {
            var comparer = (IEqualityComparer<string>)EqualityComparerEx.Reference();
            var str1 = 5.ToString();
            var str2 = 5.ToString();
            Assert.False(comparer.Equals(str1, str2));
            Assert.NotEqual(comparer.GetHashCode(str1), comparer.GetHashCode(str2));
            var str3 = str1;
            Assert.True(comparer.Equals(str1, str3));
            Assert.Equal(comparer.GetHashCode(str1), comparer.GetHashCode(str3));
            Assert.True(comparer.Equals(null, null));
            Assert.False(comparer.Equals(str1, null));
            Assert.False(comparer.Equals(null, str1));
            Assert.Equal(comparer.GetHashCode(null), comparer.GetHashCode(null));
        }

        [Fact]
        public void ComparerWorksByReferenceNonGeneric()
        {
            var comparer = (IEqualityComparer)EqualityComparerEx.Reference();
            var str1 = 5.ToString();
            var str2 = 5.ToString();
            Assert.False(comparer.Equals(str1, str2));
            Assert.NotEqual(comparer.GetHashCode(str1), comparer.GetHashCode(str2));
            var str3 = str1;
            Assert.True(comparer.Equals(str1, str3));
            Assert.Equal(comparer.GetHashCode(str1), comparer.GetHashCode(str3));
            Assert.True(comparer.Equals(null, null));
            Assert.False(comparer.Equals(str1, null));
            Assert.False(comparer.Equals(null, str1));
            Assert.Equal(comparer.GetHashCode(null), comparer.GetHashCode(null));
        }
    }
}