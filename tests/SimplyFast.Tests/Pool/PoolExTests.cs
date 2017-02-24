using NUnit.Framework;
using SimplyFast.Pool;

namespace SimplyFast.Tests.Pool
{
    [TestFixture]
    public class PoolExTests
    {
        private class Test
        {
            public static int TotalCount;
            public readonly int Count;

            public Test()
            {
                Count = TotalCount;
                TotalCount++;
            }
        }

        [Test]
        public void BasicStoresOrCreates()
        {
            var i = 0;
            var pool = PoolEx.Basic(PooledEx.Factory(() => i++));
            using (var item = pool.Get())
                Assert.AreEqual(0, item.Instance);
            using (var item0 = pool.Get())
            {
                Assert.AreEqual(0, item0.Instance);
                using (var item1 = pool.Get())
                {
                    Assert.AreEqual(1, item1.Instance);
                    item0.Dispose();
                    Assert.AreEqual(0, pool.Get().Instance);
                }
            }
        }

        [Test]
        public void NoneCreatesAlways()
        {
            var i = 0;
            var pool = PoolEx.None(PooledEx.Factory(() => i++));
            using (var item = pool.Get())
                Assert.AreEqual(0, item.Instance);
            using (var item = pool.Get())
                Assert.AreEqual(1, item.Instance);
            using (var item = pool.Get())
                Assert.AreEqual(2, item.Instance);}

        [Test]
        public void NoneCreatesAlwaysDefaultCtor()
        {
            Test.TotalCount = 0;
            var pool = PoolEx.None(PooledEx.Factory<Test>());
            using (var obj = pool.Get())
                Assert.AreEqual(0, obj.Instance.Count);
            using (var obj = pool.Get())
                Assert.AreEqual(1, obj.Instance.Count);
            using (var obj = pool.Get())
                Assert.AreEqual(2, obj.Instance.Count);
            Assert.AreEqual(3, Test.TotalCount);
        }
    }
}