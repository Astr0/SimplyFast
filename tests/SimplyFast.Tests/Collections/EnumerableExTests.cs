using System.Linq;
using Xunit;
using SimplyFast.Collections;

namespace SimplyFast.Tests.Collections
{
    
    public class EnumerableExTests
    {
        [Fact]
        public void CopyToWorks()
        {
            var i = new[] { 1, 2, 3, 4 };
            Enumerable.Empty<int>().CopyTo(i);
            Assert.True(i.SequenceEqual(new[] { 1, 2, 3, 4 }));
            EnumerableEx.CopyTo(new[] { 2, 1 }, i, 2);
            Assert.True(i.SequenceEqual(new[] { 1, 2, 2, 1 }));
        }

        [Fact]
        public void ToHashSetWorks()
        {
            var ints1 = new[] { 1, 2, 3, 4, 5, 3, 2 };
            var set1 = ints1.ToHashSet();
            Assert.True(set1.SetEquals(ints1));

            var ints2 = new[] { 1, 1, 1, 1, 1, 1 };
            var set2 = ints2.ToHashSet();
            Assert.True(set2.SetEquals(ints2));

            Assert.Equal(0, Enumerable.Empty<string>().ToHashSet().Count);
        }
    }
}