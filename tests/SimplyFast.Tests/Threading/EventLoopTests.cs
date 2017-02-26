using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using SimplyFast.Threading;

namespace SimplyFast.Tests.Threading
{
    
    public class EventLoopTests
    {
        [Fact]
        public void EventLoopEnds()
        {
            EventLoop.Run(()=>{});
        }

        [Fact]
        public void EventLoopRuns()
        {
            var i = 0;
            EventLoop.Run(() => { i++; });
            Assert.Equal(1, i);
        }

        private static async Task RecordThreads(List<int> threadIds, int milliseconds, CancellationToken token = default (CancellationToken))
        {
            // Start thread
            threadIds.Add(Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(milliseconds, token);
            // end thread
            threadIds.Add(Thread.CurrentThread.ManagedThreadId);
        }

        [Fact]
        public void EventLoopRunsOnSameThread()
        {
            var i = 0;
            var threads = new List<int>();
            EventLoop.Run(async () =>
            {
                threads.Add(Thread.CurrentThread.ManagedThreadId);
                await RecordThreads(threads, 2);
                await RecordThreads(threads, 1);
                i++;
            });
            Assert.Equal(1, i);
            Assert.Equal(5, threads.Count);
            Assert.Equal(1, threads.Distinct().Count());
        }

        [Fact]
        public void EventLoopWhenAnyAndAllWorks()
        {
            var i = 0;
            var threads = new List<int>();
            EventLoop.Run(async () =>
            {
                threads.Add(Thread.CurrentThread.ManagedThreadId);
                var tasks = new[] {RecordThreads(threads, 20), RecordThreads(threads, 1)};
                var finish = await Task.WhenAny(tasks);
                Assert.Equal(finish, tasks[1]);
                await Task.WhenAll(tasks);
                i++;
            });
            Assert.Equal(1, i);
            Assert.Equal(5, threads.Count);
            Assert.Equal(1, threads.Distinct().Count());
        }

        [Fact]
        public void EventLoopCancellationWorks()
        {
            var i = 0;
            var threads = new List<int>();
            EventLoop.Run(() =>
            {
                threads.Add(Thread.CurrentThread.ManagedThreadId);
                var cts = new CancellationTokenSource();
                var task = RecordThreads(threads, int.MaxValue, cts.Token);
                cts.Cancel();
                Assert.True(task.IsCanceled);
                i++;
            });
            Assert.Equal(1, i);
            Assert.Equal(2, threads.Count);
            Assert.Equal(1, threads.Distinct().Count());
        }

        private static async Task Throw(int milliseconds)
        {
            if (milliseconds == 0)
                throw new InvalidOperationException("ex");
            await Task.Delay(milliseconds);
            throw new InvalidOperationException("ex");
        }

        [Fact]
        public void EventLoopPreAwaitExceptionWorks()
        {
            var i = 0;
            var gotIt = false;
            EventLoop.Run(async () =>
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
            });
            Assert.Equal(1, i);
            Assert.True(gotIt);
        }

        [Fact]
        public void EventLoopPostAwaitExceptionWorks()
        {
            var i = 0;
            var gotIt = false;
            EventLoop.Run(async () =>
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
            });
            Assert.Equal(1, i);
            Assert.True(gotIt);
        }

        [Fact]
        public void EventLoopUnhandledPreExceptionWorks()
        {
            var gotIt = false;
            EventLoop.Run(async () =>
            {
                EventLoop.UnhandledException += ex => { gotIt = true; };
                await Throw(0);
            });
            Assert.True(gotIt);
        }

        [Fact]
        public void EventLoopUnhandledPostExceptionWorks()
        {
            var gotIt = false;
            EventLoop.Run(async () =>
            {
                EventLoop.UnhandledException += ex => { gotIt = true; };
                await Throw(1);
            });
            Assert.True(gotIt);
        }

        private static async Task<int> Recursive(int o)
        {
            if (o == 0)
                return 0;
            var recursive = Recursive(o - 1);
            await Task.WhenAll(Task.Delay(Math.Min(o, 100)), recursive);
            return 1 + recursive.Result;
        }

        [Fact]
        public void NoEventLoopRecursive()
        {
            var res = Recursive(800).Result;
            Assert.Equal(800, res);
        }

        [Fact]
        public void EventLoopRecursive()
        {
            var res = 0;
            EventLoop.Run(async () =>
            {
                res = await Recursive(800);
            });
            Assert.Equal(800, res);
        }
    }
}