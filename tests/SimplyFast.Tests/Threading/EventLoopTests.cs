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
            EventLoop.Run(() => { });
            EventLoop.Run(x => { }, null);
        }

        [Fact]
        public void EventLoopPassesArg()
        {
            var test = new object();
            object received = null;
            EventLoop.Run(x => { received = x; }, test);
            Assert.Equal(test, received);
        }

        [Fact]
        public void EventLoopRuns()
        {
            var i = 0;
            EventLoop.Run(() => { i++; });
            Assert.Equal(1, i);
        }


        [Fact]
        public void EventLoopRunsAsync()
        {
            var i = 0;
            EventLoop.Run(async () =>
            {
                await Task.Yield();
                i++;
            });
            Assert.Equal(1, i);
        }

        private static async Task RecordThreads(List<int> threadIds, Task waitTask)
        {
            // Start thread
            threadIds.Add(Thread.CurrentThread.ManagedThreadId);
            await waitTask;
            // end thread
            threadIds.Add(Thread.CurrentThread.ManagedThreadId);
        }

        [Fact]
        public void EventLoopRunsOnSameThread()
        {
            var threads = new List<int>();
            var i = 0;
            EventLoop.Run(async () =>
            {
                threads.Add(Thread.CurrentThread.ManagedThreadId);
                var tcs = new TaskCompletionSource<bool>();
                var rt1 = RecordThreads(threads, tcs.Task);
                var rt2 = RecordThreads(threads, tcs.Task);
                tcs.TrySetResult(true);
                await rt1;
                await rt2;
                i++;
            });
            Assert.Equal(1, i);
            Assert.Equal(5, threads.Count);
            Assert.Equal(1, threads.Distinct().Count());
        }

        [Fact]
        public void EventLoopWhenAnyAndAllWorks()
        {
            var threads = new List<int>();
            var i = 0;
            Task[] tasks = null;
            Task finish = null;
            EventLoop.Run(async () =>
            {
                threads.Add(Thread.CurrentThread.ManagedThreadId);
                var first = new TaskCompletionSource<bool>();
                var second = new TaskCompletionSource<bool>();
                tasks = new[] {RecordThreads(threads, first.Task), RecordThreads(threads, second.Task)};
                var finishTask = Task.WhenAny(tasks);
                second.TrySetResult(true);
                first.TrySetResult(true);
                finish = await finishTask;
                await Task.WhenAll(tasks);
                i++;
            });
            Assert.Equal(1, i);
            Assert.Equal(finish, tasks[1]);
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
                var tcs = new TaskCompletionSource<bool>();
                var task = RecordThreads(threads, tcs.Task);
                tcs.TrySetCanceled();
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
            var gotIt = false;
            var i = 0;
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
                ++i;
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
            await Task.WhenAll(Task.Delay(Math.Min(o, 10)), recursive);
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