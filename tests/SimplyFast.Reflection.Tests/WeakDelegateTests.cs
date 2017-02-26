using System;
using Xunit;

namespace SimplyFast.Reflection.Tests
{
    
    public class WeakDelegateTests
    {
        [Fact]
        public void EmptyWorks()
        {
            var wd1 = WeakDelegate<Func<string>>.Create();
            Assert.Null(wd1.Invoker());
            var wd2 = WeakDelegate<Action<string>>.Create();
            wd2.Invoker("test");
            var wd3 = WeakDelegate<Action<string, int>>.Create();
            wd3.Invoker("test", 1);
        }
        
        [Fact]
        public void InvokerWorksForAction1()
        {
            var i = 0;
            // ReSharper disable once AccessToModifiedClosure
            Action<int> add = x => i += x;

            var wd1 = WeakDelegate<Action<int>>.Create();
            wd1.Add(add);
            wd1.Invoker(2);
            Assert.Equal(2, i);
            wd1.Add(add);
            i = 0;
            wd1.Invoker(1);
            Assert.Equal(2, i);
        }

        [Fact]
        public void InvokerWorksForAction2()
        {
            var i = 0;
            // ReSharper disable once AccessToModifiedClosure
            Action<int, int> add = (x, y) => i += x + y;

            var wd1 = WeakDelegate<Action<int, int>>.Create();
            wd1.Add(add);
            wd1.Invoker(1, 2);
            Assert.Equal(3, i);
            wd1.Add(add);
            i = 0;
            wd1.Invoker(1, 2);
            Assert.Equal(6, i);
        }

        [Fact]
        public void InvokerWorksForFunc1()
        {
            Func<int, int> sq = x => x * x;
            Func<int, int> cube = x => x * x * x;

            var wd1 = WeakDelegate<Func<int, int>>.Create();
            wd1.Add(sq);
            Assert.Equal(4, wd1.Invoker(2));
            wd1.Add(cube);
            Assert.Equal(8, wd1.Invoker(2));
            wd1.Remove(cube);
            Assert.Equal(4, wd1.Invoker(2));
            wd1.Remove(sq);
            Assert.Equal(0, wd1.Invoker(2));
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

        [Fact]
        public void DelegateIsWeak()
        {
            var wd1 = WeakDelegate<Func<int>>.Create();
            {
                var t = new TestClass(5);
                wd1.Add(t.GetValue);
                Assert.Equal(5, wd1.Invoker());
            }
            GCEx.CollectAndWait();
            Assert.Equal(0, wd1.Invoker());
        }

        [Fact]
        public void DelegateThrowsForStupidTypes()
        {
            Assert.Throws<TypeInitializationException>(() => WeakDelegate<string>.Create());
        }
    }
}