using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using SimplyFast.Collections.Concurrent;

namespace SimplyFast.Tests.Collections.Concurrent
{
    
    public class ConcurrentGrowListTests
    {
        [Fact]
        public void WorksSingleThreaded()
        {
            var c = new ConcurrentGrowList<int> { 1, 2, 3, 4, 5 };
            Assert.Equal(5, c.Count);
            Assert.True(c.SequenceEqual(new[] { 1, 2, 3, 4, 5 }));
            var snap = c.GetSnapshot();
            Assert.Equal(5, snap.Count);
            Assert.True(snap.SequenceEqual(new[] { 1, 2, 3, 4, 5 }));
            Assert.Equal(1, c[0]);
            Assert.Equal(2, c[1]);
            Assert.Equal(3, c[2]);
            Assert.Equal(4, c[3]);
            Assert.Equal(5, c[4]);
            Assert.Equal(1, snap[0]);
            Assert.Equal(2, snap[1]);
            Assert.Equal(3, snap[2]);
            Assert.Equal(4, snap[3]);
            Assert.Equal(5, snap[4]);
        }

        [Fact]
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
                Assert.True(Enumerable.Range(0, count).SequenceEqual(c.OrderBy(x => x)));
            }
        }

        [Fact]
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
                Assert.InRange(snapshot.Count, lastCount, count);
                if (lastCount < 0)
                    lastCount = 0;
                for (var i = lastCount; i < snapshot.Count; i++)
                {
                    Assert.Equal(i, snapshot[i]);
                }
                lastCount = snapshot.Count;
            }
            Assert.True(c.SequenceEqual(Enumerable.Range(0, count)));
        }
    }
}