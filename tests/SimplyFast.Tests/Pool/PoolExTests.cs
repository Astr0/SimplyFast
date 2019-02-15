using System;
using System.Collections.Concurrent;
using Xunit;
using SimplyFast.Pool;

namespace SimplyFast.Tests.Pool
{
    
    public class PoolExTests
    {
        private static void PoolOk(Func<InitPooled<int, object>, ReturnToPoll<int>, IPool<int, object>> makePool)
        {
            var doneTestPool = makePool((x, p) => (int?) p ?? 2, null);
            using (var pooledNull = doneTestPool.Get())
                Assert.Equal(2, pooledNull.Instance);
            using (var pooledNotNull = doneTestPool.Get(3))
                Assert.Equal(3, pooledNotNull.Instance);
            

            var i = 0;
            var clean = 0;
            var pool = makePool((x, p) => x == 0 ? ++i : x, x => clean++);
            using (var item = pool.Get())
            {
                Assert.Equal(1, item.Instance);
                Assert.Equal(1, i);
                Assert.Equal(0, clean);
            }
            Assert.Equal(1, clean);

            using (var item1 = pool.Get())
            {
                Assert.Equal(1, item1.Instance);
                Assert.Equal(1, i);
                Assert.Equal(1, clean);
                using (var item2 = pool.Get())
                {
                    Assert.Equal(2, item2.Instance);
                    Assert.Equal(2, i);
                    Assert.Equal(1, clean);
                    item1.Dispose();
                    Assert.Equal(2, clean);
                    Assert.Equal(1, pool.Get().Instance);
                }
            }
        }

        [Fact]
        public void ThreadSafeOk()
        {
            PoolOk(PoolEx.ThreadSafe);
        }

        [Fact]
        public void LockingOk()
        {
            PoolOk((i, c) => PoolEx.ThreadUnsafe(i, c).Locking());
        }

        [Fact]
        public void ThreadUnsafeOk()
        {
            PoolOk(PoolEx.ThreadUnsafe);
        }

        [Fact]
        public void ConcurrentOk()
        {
            PoolOk((i, d) => PoolEx.Concurrent(i, d));
            PoolOk((i, d) => PoolEx.Concurrent(i, d , new ConcurrentQueue<int>()));
        }

        [Fact]
        public void NoneCreatesAlways()
        {
            var i = 0;
            var pool = PoolEx.None((int x, object p) => x == 0 ? i++ : x);
            using (var item = pool.Get())
                Assert.Equal(0, item.Instance);
            using (var item = pool.Get())
                Assert.Equal(1, item.Instance);
            using (var item = pool.Get())
                Assert.Equal(2, item.Instance);}

        [Fact]
        public void NoneCleanups()
        {
            var i = 0;
            var clean = 0;
            var pool = PoolEx.None((int x, object p) => x == 0 ? i++ : x, x => clean++);
            using (var x = pool.Get())
            {
                Assert.Equal(0, x.Instance);
                Assert.Equal(0, clean);
            }
            Assert.Equal(1, clean);
                
            using (var x = pool.Get())
            {
                Assert.Equal(1, x.Instance);
                Assert.Equal(1, clean);
            }
            Assert.Equal(2, clean);
            using (var x = pool.Get())
            {
                Assert.Equal(2, x.Instance);
                Assert.Equal(2, clean);
            }
            Assert.Equal(3, clean);
        }
    }
}