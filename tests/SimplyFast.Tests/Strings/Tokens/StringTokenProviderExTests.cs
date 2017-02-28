using System.Linq;
using SimplyFast.Strings.Tokens;
using Xunit;

namespace SimplyFast.Tests.Strings.Tokens
{
    public class StringTokenProviderExTests
    {
        private static void TestStorage(IStringTokenStorage storage)
        {
            Assert.Equal(0, storage.Count());
            Assert.Null(storage.Get("test"));
            var test = StringTokenEx.String("test", "1");
            storage.Add(test);
            Assert.True(storage.SequenceEqual(new []{test}));
            Assert.Equal(test, storage.Get("test"));
            var test2 = StringTokenEx.String("test", "2");
            storage.Upsert(test2);
            Assert.Equal(test2, storage.Get("test"));
            Assert.True(storage.SequenceEqual(new[] { test2 }));
            var other = StringTokenEx.String("other", "3");
            storage.Upsert(other);
            Assert.Equal(test2, storage.Get("test"));
            Assert.Equal(other, storage.Get("other"));
            Assert.True(storage.OrderBy(x => x.Name).SequenceEqual(new[] { other, test2 }));
        }

        [Fact]
        public void SequentialOk()
        {
            TestStorage(StringTokenProviderEx.Sequential());
        }

        [Fact]
        public void IndexedOk()
        {
            TestStorage(StringTokenProviderEx.Indexed());
        }

        [Fact]
        public void CombinedCombines()
        {
            var s1 = StringTokenProviderEx.Sequential();
            var s2 = StringTokenProviderEx.Sequential();
            var c = StringTokenProviderEx.Combine(s1, s2);
            Assert.Equal(0, c.Count());
            var test1 = StringTokenEx.String("test", "1");
            s2.Add(test1);
            Assert.True(c.SequenceEqual(new[] { test1 }));
            Assert.Equal(test1, c.Get("test"));

            var test2 = StringTokenEx.String("test", "2");
            s1.Add(test2);
            Assert.True(c.SequenceEqual(new[] { test2, test1 }));
            Assert.Equal(test2, c.Get("test"));

            var other = StringTokenEx.String("other", "3");
            s2.Upsert(other);
            Assert.Equal(test2, c.Get("test"));
            Assert.Equal(other, c.Get("other"));
            Assert.True(c.OrderBy(x => x.Name).SequenceEqual(new[] { other, test2, test1 }));
        }
    }
}