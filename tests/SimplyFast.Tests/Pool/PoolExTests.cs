using System;
using System.Collections.Concurrent;
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

        private static void TestPool(Func<PooledFactory<Func<IPooled<int>>>, IPool<Func<IPooled<int>>>> makePool)
        {
            var i = 0;
            var pool = makePool(PooledEx.Factory(() => i++));
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
        public void ThreadSafeOk()
        {
            TestPool(PoolEx.ThreadSafe);
        }

        [Test]
        public void ThreadSafeLockingOk()
        {
            TestPool(PoolEx.ThreadSafeLocking);
        }

        [Test]
        public void ThreadUnsafeOk()
        {
            TestPool(PoolEx.ThreadUnsafe);
        }

        [Test]
        public void ProducerConsumerOk()
        {
            TestPool(PoolEx.Concurrent);
            TestPool(f => PoolEx.Concurrent(f, new ConcurrentQueue<Func<IPooled<int>>>()));
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