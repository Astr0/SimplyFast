using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SF.Threading;

namespace SF.Tests.Threading
{
    [TestFixture]
    public class EventLoopTests
    {
        [Test]
        public void EventLoopEnds()
        {
            Assert.DoesNotThrow(()=> EventLoop.Run(()=>{}));
        }

        [Test]
        public void EventLoopRuns()
        {
            var i = 0;
            Assert.DoesNotThrow(() => EventLoop.Run(() => { i++; }));
            Assert.AreEqual(1, i);
        }

        private static async Task RecordThreads(List<int> threadIds, int milliseconds, CancellationToken token = default (CancellationToken))
        {
            // Start thread
            threadIds.Add(Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(milliseconds, token);
            // end thread
            threadIds.Add(Thread.CurrentThread.ManagedThreadId);
        }

        [Test]
        public void EventLoopRunsOnSameThread()
        {
            var i = 0;
            var threads = new List<int>();
            Assert.DoesNotThrow(() => EventLoop.Run(async () =>
            {
                threads.Add(Thread.CurrentThread.ManagedThreadId);
                await RecordThreads(threads, 2);
                await RecordThreads(threads, 1);
                i++;
            }));
            Assert.AreEqual(1, i);
            Assert.AreEqual(5, threads.Count);
            Assert.AreEqual(1, threads.Distinct().Count());
        }

        [Test]
        public void EventLoopWhenAnyAndAllWorks()
        {
            var i = 0;
            var threads = new List<int>();
            Assert.DoesNotThrow(() => EventLoop.Run(async () =>
            {
                threads.Add(Thread.CurrentThread.ManagedThreadId);
                var tasks = new[] {RecordThreads(threads, 20), RecordThreads(threads, 1)};
                var finish = await Task.WhenAny(tasks);
                Assert.AreEqual(finish, tasks[1]);
                await Task.WhenAll(tasks);
                i++;
            }));
            Assert.AreEqual(1, i);
            Assert.AreEqual(5, threads.Count);
            Assert.AreEqual(1, threads.Distinct().Count());
        }

        [Test]
        public void EventLoopCancellationWorks()
        {
            var i = 0;
            var threads = new List<int>();
            Assert.DoesNotThrow(() => EventLoop.Run(() =>
            {
                threads.Add(Thread.CurrentThread.ManagedThreadId);
                var cts = new CancellationTokenSource();
                var task = RecordThreads(threads, int.MaxValue, cts.Token);
                cts.Cancel();
                Assert.IsTrue(task.IsCanceled);
                i++;
            }));
            Assert.AreEqual(1, i);
            Assert.AreEqual(2, threads.Count);
            Assert.AreEqual(1, threads.Distinct().Count());
        }

        private static async Task Throw(int milliseconds)
        {
            if (milliseconds == 0)
                throw new InvalidOperationException("ex");
            await Task.Delay(milliseconds);
            throw new InvalidOperationException("ex");
        }

        [Test]
        public void EventLoopPreAwaitExceptionWorks()
        {
            var i = 0;
            var gotIt = false;
            Assert.DoesNotThrow(() => EventLoop.Run(async () =>
            {
                try
                {
                    await Throw(0);
                }
                catch (InvalidOperationException)
                {
                    gotIt = true;
                }
                i++;
            }));
            Assert.AreEqual(1, i);
            Assert.IsTrue(gotIt);
        }

        [Test]
        public void EventLoopPostAwaitExceptionWorks()
        {
            var i = 0;
            var gotIt = false;
            Assert.DoesNotThrow(() => EventLoop.Run(async () =>
            {
                try
                {
                    await Throw(1);
                }
                catch (InvalidOperationException)
                {
                    gotIt = true;
                }
                i++;
            }));
            Assert.AreEqual(1, i);
            Assert.IsTrue(gotIt);
        }

        [Test]
        public void EventLoopUnhandledPreExceptionWorks()
        {
            var gotIt = false;
            Assert.DoesNotThrow(() => EventLoop.Run(async () =>
            {
                EventLoop.UnhandledException += ex => { gotIt = true; };
                await Throw(0);
            }));
            Assert.IsTrue(gotIt);
        }

        [Test]
        public void EventLoopUnhandledPostExceptionWorks()
        {
            var gotIt = false;
            Assert.DoesNotThrow(() => EventLoop.Run(async () =>
            {
                EventLoop.UnhandledException += ex => { gotIt = true; };
                await Throw(1);
            }));
            Assert.IsTrue(gotIt);
        }

        public async Task<int> Recursive(int o)
        {
            if (o == 0)
                return 0;
            var recursive = Recursive(o - 1);
            await Task.WhenAll(Task.Delay(Math.Min(o, 100)), recursive);
            return 1 + recursive.Result;
        }

        [Test]
        public void NoEventLoopRecursive()
        {
            var res = Recursive(800).Result;
            Assert.AreEqual(800, res);
        }

        [Test]
        public void EventLoopRecursive()
        {
            var res = 0;
            Assert.DoesNotThrow(() => EventLoop.Run(async () =>
            {
                res = await Recursive(800);
            }));
            Assert.AreEqual(800, res);
        }
    }
}