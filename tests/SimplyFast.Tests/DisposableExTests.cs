using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SimplyFast.Disposables;
using SimplyFast.Threading;

namespace SimplyFast.Tests
{
    [TestFixture]
    public class DisposableExTests
    {
        [Test]
        public void ActionInvokesOnDispose()
        {
            var a = 1;
            IDisposable disposable;
            using (disposable = DisposableEx.Action(() => a = 3))
            {
                a = 2;
            }
            Assert.AreEqual(3, a);
            a = 5;
            disposable.Dispose();
            Assert.AreEqual(5, a);
        }

        [Test]
        public void AlwaysActionWorks()
        {
            var a = 1;
            IDisposable disposable;
            using (disposable = DisposableEx.AlwaysAction(() => a = 3))
            {
                a = 2;
            }
            Assert.AreEqual(3, a);
            a = 5;
            disposable.Dispose();
            Assert.AreEqual(3, a);
        }

        [Test]
        public void AssignDisposableWorks()
        {
            var i = 0;
            var a = DisposableEx.Assign(DisposableEx.Action(() => i++));
            Assert.AreEqual(0, i);
            var newItem = DisposableEx.Action(() => i += 10);
            a.Item = newItem;
            Assert.AreEqual(1, i);
            a.Item = newItem;
            Assert.AreEqual(1, i);
            a.Dispose();
            Assert.AreEqual(11, i);
            a.Dispose();
            Assert.AreEqual(11, i);
            a.Item = DisposableEx.Action(() => i = 0);
            Assert.AreEqual(11, i);
            a.Item = newItem;
            Assert.AreEqual(0, i);
            using (var d = DisposableEx.Assign<IDisposable>())
            {
                d.Item = null;
            }
        }

        [Test]
        public void CollectionRemoveWorks()
        {
            var list = new List<int> {1, 2};
            var d1 = DisposableEx.Remove(list, 1);
            var d2 = DisposableEx.Remove(list, 2);
            Assert.IsTrue(list.SequenceEqual(new[] {1, 2}));
            d1.Dispose();
            Assert.IsTrue(list.SequenceEqual(new[] {2}));
            d2.Dispose();
            Assert.AreEqual(0, list.Count);
            list.AddRange(new[] {1, 2});
            Assert.IsTrue(list.SequenceEqual(new[] {1, 2}));
            d1.Dispose();
            d2.Dispose();
            Assert.IsTrue(list.SequenceEqual(new[] {1, 2}));
        }

        [Test]
        public void ConcatDisposableWorks()
        {
            var i = 0;
            var j = 0;
            using (DisposableEx.Concat(DisposableEx.Action(() => i = 1), DisposableEx.Action(() => j = 1)))
            {
            }
            Assert.AreEqual(1, i);
            Assert.AreEqual(1, j);
        }

        [Test]
        public void DisposeOnFinalizeWorks()
        {
            var i = 0;
            DisposableEx.Action(() => i++).DisposeOnFinalize();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.AreEqual(1, i);
        }

        [Test]
        public void KeepAliveTest()
        {
            WeakReference wr;
            {
                wr = new WeakReference(new object());
                using (DisposableEx.Null().KeepAlive(wr.Target))
                {
                    GCEx.CollectAndWait();
                    Assert.IsTrue(wr.IsAlive);
                }
            }
            GCEx.CollectAndWait();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
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
                Assert.AreEqual(ts.Value, threadId);
                Assert.AreNotEqual(realThreadId, threadId);
            });
        }

        [Test]
        public void DisposeCollectionWorks()
        {
            var a = 0;
            var b = 0;
            var ad = DisposableEx.AlwaysAction(() => a++);
            var bd = DisposableEx.AlwaysAction(() => b++);
            var arr = new[] {ad, bd};
            var list = new List<IDisposable>{ ad };
            list.AddAction(() => b++);
            arr.Dispose();
            Assert.AreEqual(2, arr.Length);
            Assert.AreEqual(1, a);
            Assert.AreEqual(1, b);
            ((IEnumerable<IDisposable>)list).Dispose();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(2, a);
            Assert.AreEqual(2, b);
            list.Dispose();
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(3, a);
            Assert.AreEqual(2, b);
        }
    }
}