using System.Linq;
using NUnit.Framework;
using SF.Collections;

namespace SF.Tests.Collections
{
    [TestFixture]
    public class EnumerableExTests
    {
        [Test]
        public void CopyToWorks()
        {
            var i = new[] { 1, 2, 3, 4 };
            Enumerable.Empty<int>().CopyTo(i);
            Assert.IsTrue(i.SequenceEqual(new[] { 1, 2, 3, 4 }));
            EnumerableEx.CopyTo(new[] { 2, 1 }, i, 2);
            Assert.IsTrue(i.SequenceEqual(new[] { 1, 2, 2, 1 }));
        }

        [Test]
        public void ToHashSetWorks()
        {
            var ints1 = new[] { 1, 2, 3, 4, 5, 3, 2 };
            var set1 = ints1.ToHashSet();
            Assert.IsTrue(set1.SetEquals(ints1));

            var ints2 = new[] { 1, 1, 1, 1, 1, 1 };
            var set2 = ints2.ToHashSet();
            Assert.IsTrue(set2.SetEquals(ints2));

            Assert.AreEqual(0, Enumerable.Empty<string>().ToHashSet().Count);
        }
    }
}