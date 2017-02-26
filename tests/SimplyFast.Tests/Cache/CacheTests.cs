using Xunit;
using SimplyFast.Cache;

namespace SimplyFast.Tests.Cache
{
    
    public class CacheTests
    {
        private static void TestCache(ICache<int, string> cache)
        {
            Assert.False(cache.TryGetValue(1, out string str0));
            Assert.Null(str0);
            var str1 = cache.GetOrAdd(1, MakeValue);
            Assert.Equal("1", str1);
            var str2 = cache.GetOrAdd(1, MakeValue);
            Assert.Equal("1", str2);
            Assert.True(ReferenceEquals(str1, str2));
            var str3 = cache.GetOrAdd(1, MakeValue, out bool added);
            Assert.False(added);
            Assert.Equal("1", str3);
            Assert.True(ReferenceEquals(str1, str3));
            Assert.True(cache.TryGetValue(1, out string str4));
            Assert.Equal("1", str4);
            Assert.True(ReferenceEquals(str1, str4));
            cache.Clear();
            Assert.False(cache.TryGetValue(1, out string str5));
            Assert.Null(str5);
            var str6 = cache.GetOrAdd(1, MakeValue, out added);
            Assert.True(added);
            Assert.Equal("1", str6);
            Assert.False(ReferenceEquals(str1, str6));
            var str7 = cache.GetOrAdd(1, MakeValue);
            Assert.Equal("1", str7);
            Assert.True(ReferenceEquals(str6, str7));
            const string test = "test";
            cache.Upsert(1, test);
            var str8 = cache.GetOrAdd(1, MakeValue);
            Assert.Equal(test, str8);
            Assert.True(ReferenceEquals(test, str8));
        }

        private static string MakeValue(int key)
        {
            return key.ToString();
        }

        [Fact]
        public void NoneDoesntCache()
        {
            var cache = CacheEx.None<int, string>();
            var str1 = cache.GetOrAdd(1, MakeValue);
            Assert.Equal("1", str1);
            var str2 = cache.GetOrAdd(1, MakeValue);
            Assert.Equal("1", str2);
            Assert.False(ReferenceEquals(str1, str2));
            cache.Upsert(1, "test");
            var str3 = cache.GetOrAdd(1, MakeValue, out bool added);
            Assert.True(added);
            Assert.Equal("1", str3);
            Assert.False(ReferenceEquals(str1, str3));
            Assert.False(ReferenceEquals(str2, str3));
            Assert.False(cache.TryGetValue(1, out string str4));
            Assert.Null(str4);
            cache.Clear();
        }

        [Fact]
        public void ThreadSafeOk()
        {
            TestCache(CacheEx.ThreadSafe<int, string>());
        }
        [Fact]
        public void ThreadSafeLockingOk()
        {
            TestCache(CacheEx.ThreadSafeLocking<int, string>());
        }
        [Fact]
        public void ThreadUnsafeOk()
        {
            TestCache(CacheEx.ThreadUnsafe<int, string>());
        }
#if CONCURRENT
        [Fact]
        public void ConcurrentOk()
        {
            TestCache(CacheEx.Concurrent<int, string>());
        }
#endif
    }
}