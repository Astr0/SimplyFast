using System;
using System.Collections.Generic;
using Xunit;
using SimplyFast.Pool;

namespace SimplyFast.Tests.Pool
{
    
    public class PooledExTests
    {
        [Fact]
        public void ExceptionsOk()
        {
            var inPoolException = PooledEx.InPoolException();
            var notInPoolException = PooledEx.NotInPoolException();
            Assert.IsType<InvalidOperationException>(inPoolException);
            Assert.IsType<InvalidOperationException>(notInPoolException);
            Assert.NotNull(inPoolException);
            Assert.NotNull(notInPoolException);
            Assert.NotEqual(inPoolException, notInPoolException);
            Assert.NotEqual(inPoolException, PooledEx.InPoolException());
            Assert.NotEqual(inPoolException, PooledEx.NotInPoolException());
        }

        [Fact]
        public void NotPooledOk()
        {
            var obj = new object();
            var np = PooledEx.NotPooled(obj);
            Assert.Equal(obj, np.Instance);
            np.Dispose();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<InvalidOperationException>(() => Equals(np.Instance, null));
        }

        [Fact]
        public void MostBasicFactoryOk()
        {
            var factory = PooledEx.Factory<object>();
            var returned = 0;
            ReturnToPool<Func<object>> returnToPool = x => returned++;
            using (var pooled1 = factory(returnToPool)())
            {
                Assert.NotNull(pooled1.Instance);
                using (var pooled2 = factory(returnToPool)())
                {
                    Assert.NotNull(pooled2.Instance);
                    Assert.NotEqual(pooled1.Instance, pooled2.Instance);
                }
                Assert.Equal(1, returned);
                pooled1.Dispose();
                Assert.Equal(2, returned);
            }
            Assert.Equal(2, returned);
        }

        [Fact]
        public void FactoryWithCleanupOk()
        {
            var cleaned = new List<object>();
            var returned = 0;
            var expectReturned = 0;
            var factory = PooledEx.Factory<object>(x =>
            {
                // ReSharper disable once AccessToModifiedClosure
                Assert.Equal(expectReturned, returned);
                cleaned.Add(x);
            });
            ReturnToPool<Func<object>> returnToPool = x => returned++;
            object i1, i2;
            using (var pooled1 = factory(returnToPool)())
            {
                i1 = pooled1.Instance;
                Assert.Equal(0, cleaned.Count);
                Assert.NotNull(pooled1.Instance);
                using (var pooled2 = factory(returnToPool)())
                {
                    i2 = pooled2.Instance;
                    Assert.Equal(0, cleaned.Count);
                    Assert.NotNull(pooled2.Instance);
                    Assert.NotEqual(pooled1.Instance, pooled2.Instance);
                    expectReturned = 0;
                }
                Assert.Equal(i2, cleaned[0]);
                Assert.Equal(1, returned);
                expectReturned = 1;
            }
            Assert.Equal(i2, cleaned[0]);
            Assert.Equal(i1, cleaned[1]);
            Assert.Equal(2, returned);
        }

        [Fact]
        public void FactoryWithActivatorOk()
        {
            var i = 0;
            var factory = PooledEx.Factory(() => i++);
            var returned = 0;
            ReturnToPool<Func<object>> returnToPool = x => returned++;
            using (var pooled1 = factory(returnToPool)())
            {
                Assert.Equal(0, pooled1.Instance);
                Assert.Equal(1, i);
                using (var pooled2 = factory(returnToPool)())
                {
                    Assert.Equal(1, pooled2.Instance);
                    Assert.Equal(2, i);
                }
                Assert.Equal(1, returned);
                pooled1.Dispose();
                Assert.Equal(2, returned);
            }
            Assert.Equal(2, returned);
            Assert.Equal(2, i);
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

        [Fact]
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
                Assert.Equal(1, pooled1.Instance.Value);
                using (var pooled2 = factory(returnToPool)(25))
                {
                    Assert.Equal(25, pooled2.Instance.Value);
                }
                Assert.Equal(1, returned);
                pooled1.Dispose();
                Assert.Equal(2, returned);
            }
            Assert.Equal(2, returned);
        }

        [Fact]
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
                Assert.Equal(1, pooled1.Instance.Value);
                using (var pooled2 = factory(returnToPool)(25))
                {
                    Assert.Equal(25, pooled2.Instance.Value);
                }
                Assert.Equal(1, returned);
                pooled1.Dispose();
                Assert.Equal(2, returned);
                using (var pooled3 = factory(returnToPool)(null))
                {
                    // activator value
                    Assert.Equal(11, pooled3.Instance.Value);
                }
                Assert.Equal(3, returned);
            }
            Assert.Equal(3, returned);
        }
    }
}