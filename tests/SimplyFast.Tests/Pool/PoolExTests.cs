using NUnit.Framework;
using SF.Pool;

namespace SF.Tests.Pool
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
            var pool = PoolEx.Basic(() => i++);
            Assert.AreEqual(0, pool.Get());
            pool.Return(0);
            Assert.AreEqual(0, pool.Get());
            Assert.AreEqual(1, pool.Get());
            pool.Return(0);
            Assert.AreEqual(0, pool.Get());
        }

        [Test]
        public void NoneCreatesAlways()
        {
            var i = 0;
            var pool = PoolEx.None(() => i++);
            Assert.AreEqual(0, pool.Get());
            pool.Return(0);
            Assert.AreEqual(1, pool.Get());
            pool.Return(1);
            Assert.AreEqual(2, pool.Get());
            pool.Return(2);
        }

        [Test]
        public void NoneCreatesAlwaysDefaultCtor()
        {
            Test.TotalCount = 0;
            var pool = PoolEx.None<Test>();
            var obj = pool.Get();
            Assert.AreEqual(0, obj.Count);
            pool.Return(obj);
            obj = pool.Get();
            Assert.AreEqual(1, obj.Count);
            pool.Return(obj);
            obj = pool.Get();
            Assert.AreEqual(2, obj.Count);
            pool.Return(obj);
            Assert.AreEqual(3, Test.TotalCount);
        }
    }
}