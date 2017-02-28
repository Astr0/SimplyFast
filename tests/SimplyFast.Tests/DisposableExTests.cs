using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using SimplyFast.Disposables;
using SimplyFast.Threading;

namespace SimplyFast.Tests
{
    
    public class DisposableExTests
    {
        [Fact]
        public void ActionInvokesOnDispose()
        {
            var a = 1;
            IDisposable disposable;
            using (disposable = DisposableEx.Action(() => a = 3))
            {
                a = 2;
            }
            Assert.Equal(3, a);
            a = 5;
            disposable.Dispose();
            Assert.Equal(5, a);
        }

        [Fact]
        public void AlwaysActionWorks()
        {
            var a = 1;
            IDisposable disposable;
            using (disposable = DisposableEx.AlwaysAction(() => a = 3))
            {
                a = 2;
            }
            Assert.Equal(3, a);
            a = 5;
            disposable.Dispose();
            Assert.Equal(3, a);
        }

        [Fact]
        public void AssignDisposableWorks()
        {
            var i = 0;
            var a = DisposableEx.Assign(DisposableEx.Action(() => i++));
            Assert.Equal(0, i);
            var newItem = DisposableEx.Action(() => i += 10);
            a.Item = newItem;
            Assert.Equal(1, i);
            a.Item = newItem;
            Assert.Equal(1, i);
            a.Dispose();
            Assert.Equal(11, i);
            a.Dispose();
            Assert.Equal(11, i);
            a.Item = DisposableEx.Action(() => i = 0);
            Assert.Equal(11, i);
            a.Item = newItem;
            Assert.Equal(0, i);
            using (var d = DisposableEx.Assign<IDisposable>())
            {
                d.Item = null;
            }
        }

        [Fact]
        public void CollectionRemoveWorks()
        {
            var list = new List<int> {1, 2};
            var d1 = DisposableEx.Remove(list, 1);
            var d2 = DisposableEx.Remove(list, 2);
            Assert.True(list.SequenceEqual(new[] {1, 2}));
            d1.Dispose();
            Assert.True(list.SequenceEqual(new[] {2}));
            d2.Dispose();
            Assert.Equal(0, list.Count);
            list.AddRange(new[] {1, 2});
            Assert.True(list.SequenceEqual(new[] {1, 2}));
            d1.Dispose();
            d2.Dispose();
            Assert.True(list.SequenceEqual(new[] {1, 2}));
        }

        [Fact]
        public void ConcatDisposableWorks()
        {
            var i = 0;
            var j = 0;
            using (DisposableEx.Concat(DisposableEx.Action(() => i = 1), DisposableEx.Action(() => j = 1)))
            {
            }
            Assert.Equal(1, i);
            Assert.Equal(1, j);
        }

        [Fact]
        public void DisposeOnFinalizeWorks()
        {
            var i = 0;
            DisposableEx.Action(() => i++).DisposeOnFinalize();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.Equal(1, i);
        }

        [Fact]
        public void KeepAliveTest()
        {
            WeakReference wr;
            {
                wr = new WeakReference(new object());
                using (DisposableEx.Null().KeepAlive(wr.Target))
                {
                    GCEx.CollectAndWait();
                    Assert.True(wr.IsAlive);
                }
            }
            GCEx.CollectAndWait();
            Assert.False(wr.IsAlive);
        }

        [Fact]
        public void UseContextWorks()
        {
            var defaultTaskScheduler = TaskScheduler.Default;
            var v = 0;
            var ts = new ThreadLocal<int>(() => v++);
            EventLoop.Run(() =>
            {
                var sc = SynchronizationContext.Current;
                var done = false;

                var threadId = -1;
                var realThreadId = -1;
                var disp = DisposableEx.Action(() => threadId = ts.Value).UseContext(sc);
                Task.Factory.StartNew(()=>
                {
                    realThreadId = ts.Value;
                    disp.Dispose();
                    done = true;
                }, CancellationToken.None, TaskCreationOptions.None, defaultTaskScheduler);
                
                while (!done)
                    EventLoop.DoEvents();
                Assert.Equal(ts.Value, threadId);
                Assert.NotEqual(realThreadId, threadId);
            });
        }

        [Fact]
        public void DisposeCollectionWorks()
        {
            var a = 0;
            var b = 0;
            var ad = DisposableEx.AlwaysAction(() => a++);
            var bd = DisposableEx.AlwaysAction(() => b++);
            var arr = new[] {ad, bd};
            var list = new List<IDisposable>{ ad };
            list.AddAction(() => b++);
            DisposableEx.Dispose(arr);
            Assert.Equal(2, arr.Length);
            Assert.Equal(1, a);
            Assert.Equal(1, b);
            DisposableEx.Dispose((IEnumerable<IDisposable>)list);
            Assert.Equal(2, list.Count);
            Assert.Equal(2, a);
            Assert.Equal(2, b);
            DisposableEx.Dispose(list);
            Assert.Equal(0, list.Count);
            Assert.Equal(3, a);
            Assert.Equal(2, b);
        }
    }
}