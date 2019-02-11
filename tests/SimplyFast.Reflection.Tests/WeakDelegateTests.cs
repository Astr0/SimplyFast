using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Xunit;

namespace SimplyFast.Reflection.Tests
{
    public abstract class WeakDelegateTestsBase: IDisposable
    {
        private static readonly object _lock = new object();
        private readonly bool _useEmit;

        protected WeakDelegateTestsBase(bool useEmit)
        {
            Monitor.Enter(_lock);
            _useEmit = useEmit;
            WeakDelegate.UseEmit = useEmit;
        }

        [Fact]
        public void UseEmitApplied()
        {
            Assert.Equal(_useEmit, WeakDelegate.UseEmit);
        }

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

        private class SomeClass
        {
            public SomeClass(int value)
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
                var t = new SomeClass(5);
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

        public void Dispose()
        {
            Monitor.Exit(_lock);
        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class WeakDelegateEmitTests: WeakDelegateTestsBase
    {
        public WeakDelegateEmitTests() : base(true)
        {
        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class WeakDelegateExpressionsTests: WeakDelegateTestsBase
    {
        public WeakDelegateExpressionsTests() : base(false)
        {
        }
    }
}