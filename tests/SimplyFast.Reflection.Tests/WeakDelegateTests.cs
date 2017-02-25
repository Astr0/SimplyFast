using System;
using NUnit.Framework;

namespace SimplyFast.Reflection.Tests
{
    [TestFixture]
    public class WeakDelegateTests
    {
        [Test]
        public void EmptyWorks()
        {
            var wd1 = WeakDelegate<Func<string>>.Create();
            Assert.IsNull(wd1.Invoker());
            var wd2 = WeakDelegate<Action<string>>.Create();
            Assert.DoesNotThrow(() => wd2.Invoker("test"));
            var wd3 = WeakDelegate<Action<string, int>>.Create();
            Assert.DoesNotThrow(() => wd3.Invoker("test", 1));
        }
        
        [Test]
        public void InvokerWorksForAction1()
        {
            var i = 0;
            // ReSharper disable once AccessToModifiedClosure
            Action<int> add = x => i += x;

            var wd1 = WeakDelegate<Action<int>>.Create();
            wd1.Add(add);
            wd1.Invoker(2);
            Assert.AreEqual(2, i);
            wd1.Add(add);
            i = 0;
            wd1.Invoker(1);
            Assert.AreEqual(2, i);
        }

        [Test]
        public void InvokerWorksForAction2()
        {
            var i = 0;
            // ReSharper disable once AccessToModifiedClosure
            Action<int, int> add = (x, y) => i += x + y;

            var wd1 = WeakDelegate<Action<int, int>>.Create();
            wd1.Add(add);
            wd1.Invoker(1, 2);
            Assert.AreEqual(3, i);
            wd1.Add(add);
            i = 0;
            wd1.Invoker(1, 2);
            Assert.AreEqual(6, i);
        }

        [Test]
        public void InvokerWorksForFunc1()
        {
            Func<int, int> sq = x => x * x;
            Func<int, int> cube = x => x * x * x;

            var wd1 = WeakDelegate<Func<int, int>>.Create();
            wd1.Add(sq);
            Assert.AreEqual(4, wd1.Invoker(2));
            wd1.Add(cube);
            Assert.AreEqual(8, wd1.Invoker(2));
            wd1.Remove(cube);
            Assert.AreEqual(4, wd1.Invoker(2));
            wd1.Remove(sq);
            Assert.AreEqual(0, wd1.Invoker(2));
        }

        class TestClass
        {
            public TestClass(int value)
            {
                _value = value;
            }

            private readonly int _value;

            public int GetValue()
            {
                return _value;
            }
        }

        [Test]
        public void DelegateIsWeak()
        {
            var wd1 = WeakDelegate<Func<int>>.Create();
            {
                var t = new TestClass(5);
                wd1.Add(t.GetValue);
                Assert.AreEqual(5, wd1.Invoker());
            }
            GCEx.CollectAndWait();
            Assert.AreEqual(0, wd1.Invoker());
        }

        [Test]
        public void DelegateThrowsForStupidTypes()
        {
            Assert.Throws<TypeInitializationException>(() => WeakDelegate<string>.Create());
        }
    }
}