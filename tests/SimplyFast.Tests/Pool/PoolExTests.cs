using System;
using System.Collections.Concurrent;
using Xunit;
using SimplyFast.Pool;

namespace SimplyFast.Tests.Pool
{
    
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

        private static void PoolOk(Func<PooledFactory<Func<IPooled<int>>>, IPool<Func<IPooled<int>>>> makePool)
        {
            var i = 0;
            var pool = makePool(PooledEx.Factory(() => i++));
            using (var item = pool.Get())
                Assert.Equal(0, item.Instance);
            using (var item0 = pool.Get())
            {
                Assert.Equal(0, item0.Instance);
                using (var item1 = pool.Get())
                {
                    Assert.Equal(1, item1.Instance);
                    item0.Dispose();
                    Assert.Equal(0, pool.Get().Instance);
                }
            }
        }

        [Fact]
        public void ThreadSafeOk()
        {
            PoolOk(PoolEx.ThreadSafe);
        }

        [Fact]
        public void ThreadSafeLockingOk()
        {
            PoolOk(PoolEx.ThreadSafeLocking);
        }

        [Fact]
        public void ThreadUnsafeOk()
        {
            PoolOk(PoolEx.ThreadUnsafe);
        }

        [Fact]
        public void ConcurrentOk()
        {
            PoolOk(PoolEx.Concurrent);
            PoolOk(f => PoolEx.Concurrent(f, new ConcurrentQueue<Func<IPooled<int>>>()));
        }

        [Fact]
        public void NoneCreatesAlways()
        {
            var i = 0;
            var pool = PoolEx.None(PooledEx.Factory(() => i++));
            using (var item = pool.Get())
                Assert.Equal(0, item.Instance);
            using (var item = pool.Get())
                Assert.Equal(1, item.Instance);
            using (var item = pool.Get())
                Assert.Equal(2, item.Instance);}

        [Fact]
        public void NoneCreatesAlwaysDefaultCtor()
        {
            Test.TotalCount = 0;
            var pool = PoolEx.None(PooledEx.Factory<Test>());
            using (var obj = pool.Get())
                Assert.Equal(0, obj.Instance.Count);
            using (var obj = pool.Get())
                Assert.Equal(1, obj.Instance.Count);
            using (var obj = pool.Get())
                Assert.Equal(2, obj.Instance.Count);
            Assert.Equal(3, Test.TotalCount);
        }
    }
}