using NUnit.Framework;
using SimplyFast.Cache;

namespace SimplyFast.Tests.Cache
{
    [TestFixture]
    public class CacheTests
    {
        private static void TestCache(ICache<int, string> cache)
        {
            Assert.IsFalse(cache.TryGetValue(1, out string str0));
            Assert.IsNull(str0);
            var str1 = cache.GetOrAdd(1, MakeValue);
            Assert.AreEqual("1", str1);
            var str2 = cache.GetOrAdd(1, MakeValue);
            Assert.AreEqual("1", str2);
            Assert.IsTrue(ReferenceEquals(str1, str2));
            var str3 = cache.GetOrAdd(1, MakeValue, out bool added);
            Assert.IsFalse(added);
            Assert.AreEqual("1", str3);
            Assert.IsTrue(ReferenceEquals(str1, str3));
            Assert.IsTrue(cache.TryGetValue(1, out string str4));
            Assert.AreEqual("1", str4);
            Assert.IsTrue(ReferenceEquals(str1, str4));
            cache.Clear();
            Assert.IsFalse(cache.TryGetValue(1, out string str5));
            Assert.IsNull(str5);
            var str6 = cache.GetOrAdd(1, MakeValue, out added);
            Assert.IsTrue(added);
            Assert.AreEqual("1", str6);
            Assert.IsFalse(ReferenceEquals(str1, str6));
            var str7 = cache.GetOrAdd(1, MakeValue);
            Assert.AreEqual("1", str7);
            Assert.IsTrue(ReferenceEquals(str6, str7));
            const string test = "test";
            cache.Upsert(1, test);
            var str8 = cache.GetOrAdd(1, MakeValue);
            Assert.AreEqual(test, str8);
            Assert.IsTrue(ReferenceEquals(test, str8));
        }

        private static string MakeValue(int key)
        {
            return key.ToString();
        }

        [Test]
        public void NoneDoesntCache()
        {
            var cache = CacheEx.None<int, string>();
            var str1 = cache.GetOrAdd(1, MakeValue);
            Assert.AreEqual("1", str1);
            var str2 = cache.GetOrAdd(1, MakeValue);
            Assert.AreEqual("1", str2);
            Assert.IsFalse(ReferenceEquals(str1, str2));
            cache.Upsert(1, "test");
            var str3 = cache.GetOrAdd(1, MakeValue, out bool added);
            Assert.IsTrue(added);
            Assert.AreEqual("1", str3);
            Assert.IsFalse(ReferenceEquals(str1, str3));
            Assert.IsFalse(ReferenceEquals(str2, str3));
            Assert.IsFalse(cache.TryGetValue(1, out string str4));
            Assert.IsNull(str4);
            Assert.DoesNotThrow(() => cache.Clear());
        }

        [Test]
        public void ThreadSafeOk()
        {
            TestCache(CacheEx.ThreadSafe<int, string>());
        }
        [Test]
        public void ThreadSafeLockingOk()
        {
            TestCache(CacheEx.ThreadSafeLocking<int, string>());
        }
        [Test]
        public void ThreadUnsafeOk()
        {
            TestCache(CacheEx.ThreadUnsafe<int, string>());
        }
        [Test]
        public void ConcurrentOk()
        {
            TestCache(CacheEx.Concurrent<int, string>());
        }
    }
}