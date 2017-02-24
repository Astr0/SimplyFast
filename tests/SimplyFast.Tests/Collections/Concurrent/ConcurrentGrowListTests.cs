using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SimplyFast.Collections.Concurrent;

namespace SimplyFast.Tests.Collections.Concurrent
{
    [TestFixture]
    public class ConcurrentGrowListTests
    {
        [Test]
        public void WorksSingleThreaded()
        {
            var c = new ConcurrentGrowList<int> { 1, 2, 3, 4, 5 };
            Assert.AreEqual(5, c.Count);
            Assert.IsTrue(c.SequenceEqual(new[] { 1, 2, 3, 4, 5 }));
            var snap = c.GetSnapshot();
            Assert.AreEqual(5, snap.Count);
            Assert.IsTrue(snap.SequenceEqual(new[] { 1, 2, 3, 4, 5 }));
            Assert.AreEqual(1, c[0]);
            Assert.AreEqual(2, c[1]);
            Assert.AreEqual(3, c[2]);
            Assert.AreEqual(4, c[3]);
            Assert.AreEqual(5, c[4]);
            Assert.AreEqual(1, snap[0]);
            Assert.AreEqual(2, snap[1]);
            Assert.AreEqual(3, snap[2]);
            Assert.AreEqual(4, snap[3]);
            Assert.AreEqual(5, snap[4]);
        }

        [Test]
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public void MultiThreadedAddOk()
        {
            const int threadCount = 1000;
            const int threads = 10;
            const int count = threads * threadCount;
            var c = new ConcurrentGrowList<int>();
            using (var start = new ManualResetEvent(false))
            using (var finish = new CountdownEvent(threads))
            {
                for (var t = 0; t < threads; t++)
                {
                    var id = t;
                    Task.Factory.StartNew(() =>
                    {
                        start.WaitOne();
                        var startIndex = id * threadCount;
                        for (var i = 0; i < threadCount; i++)
                        {
                            c.Add(startIndex + i);
                        }
                        finish.Signal();
                    });
                }
                start.Set();
                finish.Wait();
                Assert.IsTrue(Enumerable.Range(0, count).SequenceEqual(c.OrderBy(x => x)));
            }
        }

        [Test]
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public void MultiThreadedReadOk()
        {
            const int count = 100000;
            var c = new ConcurrentGrowList<int>();
            var snapshots = new List<ConcurrentGrowList<int>.Snapshot>();
            var finish = false;
            using (var start = new ManualResetEvent(false))
            {
                var thread = new Thread(() =>
                {
                    start.WaitOne();
                    for (var i = 0; i < count; i++)
                    {
                        c.Add(i);
                    }
                    finish = true;
                });
                thread.Start();
                snapshots.Add(c.GetSnapshot());
                start.Set();
                while (!finish)
                {
                    var snap = c.GetSnapshot();
                    if (snap.Count == snapshots[snapshots.Count - 1].Count)
                        continue;
                    snapshots.Add(snap);
                }
            }
            var lastCount = -1;
            foreach (var snapshot in snapshots)
            {
                Assert.Greater(snapshot.Count, lastCount);
                if (lastCount < 0)
                    lastCount = 0;
                for (var i = lastCount; i < snapshot.Count; i++)
                {
                    Assert.AreEqual(i, snapshot[i]);
                }
                lastCount = snapshot.Count;
            }
            Assert.IsTrue(c.SequenceEqual(Enumerable.Range(0, count)));
        }
    }
}