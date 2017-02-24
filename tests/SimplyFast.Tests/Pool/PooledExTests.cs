using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimplyFast.Pool;

namespace SimplyFast.Tests.Pool
{
    [TestFixture]
    public class PooledExTests
    {
        [Test]
        public void ExceptionsOk()
        {
            var inPoolException = PooledEx.InPoolException();
            var notInPoolException = PooledEx.NotInPoolException();
            Assert.IsInstanceOf<InvalidOperationException>(inPoolException);
            Assert.IsInstanceOf<InvalidOperationException>(notInPoolException);
            Assert.IsNotNull(inPoolException);
            Assert.IsNotNull(notInPoolException);
            Assert.AreNotEqual(inPoolException, notInPoolException);
            Assert.AreNotEqual(inPoolException, PooledEx.InPoolException());
            Assert.AreNotEqual(inPoolException, PooledEx.NotInPoolException());
        }

        [Test]
        public void NotPooledOk()
        {
            var obj = new object();
            var np = PooledEx.NotPooled(obj);
            Assert.AreEqual(obj, np.Instance);
            np.Dispose();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<InvalidOperationException>(() => Equals(np.Instance, null));
        }

        [Test]
        public void MostBasicFactoryOk()
        {
            var factory = PooledEx.Factory<object>();
            var returned = 0;
            ReturnToPool<Func<object>> returnToPool = x => returned++;
            using (var pooled1 = factory(returnToPool)())
            {
                Assert.IsNotNull(pooled1.Instance);
                using (var pooled2 = factory(returnToPool)())
                {
                    Assert.IsNotNull(pooled2.Instance);
                    Assert.AreNotEqual(pooled1.Instance, pooled2.Instance);
                }
                Assert.AreEqual(1, returned);
                pooled1.Dispose();
                Assert.AreEqual(2, returned);
            }
            Assert.AreEqual(2, returned);
        }

        [Test]
        public void FactoryWithCleanupOk()
        {
            var cleaned = new List<object>();
            var returned = 0;
            var expectReturned = 0;
            var factory = PooledEx.Factory<object>(x =>
            {
                // ReSharper disable once AccessToModifiedClosure
                Assert.AreEqual(expectReturned, returned);
                cleaned.Add(x);
            });
            ReturnToPool<Func<object>> returnToPool = x => returned++;
            object i1, i2;
            using (var pooled1 = factory(returnToPool)())
            {
                i1 = pooled1.Instance;
                Assert.AreEqual(0, cleaned.Count);
                Assert.IsNotNull(pooled1.Instance);
                using (var pooled2 = factory(returnToPool)())
                {
                    i2 = pooled2.Instance;
                    Assert.AreEqual(0, cleaned.Count);
                    Assert.IsNotNull(pooled2.Instance);
                    Assert.AreNotEqual(pooled1.Instance, pooled2.Instance);
                    expectReturned = 0;
                }
                Assert.AreEqual(i2, cleaned[0]);
                Assert.AreEqual(1, returned);
                expectReturned = 1;
            }
            Assert.AreEqual(i2, cleaned[0]);
            Assert.AreEqual(i1, cleaned[1]);
            Assert.AreEqual(2, returned);
        }

        [Test]
        public void FactoryWithActivatorOk()
        {
            var i = 0;
            var factory = PooledEx.Factory(() => i++);
            var returned = 0;
            ReturnToPool<Func<object>> returnToPool = x => returned++;
            using (var pooled1 = factory(returnToPool)())
            {
                Assert.AreEqual(0, pooled1.Instance);
                Assert.AreEqual(1, i);
                using (var pooled2 = factory(returnToPool)())
                {
                    Assert.AreEqual(1, pooled2.Instance);
                    Assert.AreEqual(2, i);
                }
                Assert.AreEqual(1, returned);
                pooled1.Dispose();
                Assert.AreEqual(2, returned);
            }
            Assert.AreEqual(2, returned);
            Assert.AreEqual(2, i);
        }

        private class Test
        {
            public Test(int value)
            {
                Value = value;
            }

            public Test()
            {
            }

            public int Value;
        }

        [Test]
        public void CustomFactoryOk()
        {
            var factory = PooledEx.Factory<Test, Func<int, IPooled<Test>>>(get => x =>
            {
                var pooled = get();
                pooled.Instance.Value = x;
                return pooled;
            });
            var returned = 0;
            ReturnToPool<Func<int, IPooled<Test>>> returnToPool = x => returned++;
            using (var pooled1 = factory(returnToPool)(1))
            {
                Assert.AreEqual(1, pooled1.Instance.Value);
                using (var pooled2 = factory(returnToPool)(25))
                {
                    Assert.AreEqual(25, pooled2.Instance.Value);
                }
                Assert.AreEqual(1, returned);
                pooled1.Dispose();
                Assert.AreEqual(2, returned);
            }
            Assert.AreEqual(2, returned);
        }

        [Test]
        public void CustomFactoryActivatorOk()
        {
            var factory = PooledEx.Factory<Test, Func<int?, IPooled<Test>>>(() => new Test(11), get => x =>
            {
                var pooled = get();
                if (x.HasValue)
                    pooled.Instance.Value = x.Value;
                return pooled;
            });
            var returned = 0;
            ReturnToPool<Func<int?, IPooled<Test>>> returnToPool = x => returned++;
            using (var pooled1 = factory(returnToPool)(1))
            {
                Assert.AreEqual(1, pooled1.Instance.Value);
                using (var pooled2 = factory(returnToPool)(25))
                {
                    Assert.AreEqual(25, pooled2.Instance.Value);
                }
                Assert.AreEqual(1, returned);
                pooled1.Dispose();
                Assert.AreEqual(2, returned);
                using (var pooled3 = factory(returnToPool)(null))
                {
                    // activator value
                    Assert.AreEqual(11, pooled3.Instance.Value);
                }
                Assert.AreEqual(3, returned);
            }
            Assert.AreEqual(3, returned);
        }
    }
}