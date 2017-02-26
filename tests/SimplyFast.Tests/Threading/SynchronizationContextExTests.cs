using System;
using System.Threading;
using Xunit;
using SimplyFast.Threading;

namespace SimplyFast.Tests.Threading
{
    
    public class SynchronizationContextExTests
    {
        private static void Test(Action<SynchronizationContext> action)
        {
            var done = false;
            EventLoop.Run(() =>
            {
                action(SynchronizationContext.Current);
                done = true;
            });
            Assert.True(done);
        }

        [Fact]
        public void SendWorks()
        {
            Test(sc =>
            {
                var state = 0;
                sc.Send(5, x =>
                {
                    state = x;
                });
                Assert.Equal(5, state);
                Assert.Equal(7, sc.Send(2, x => state + x));
                Assert.Equal(5, state);
                Assert.Equal(5, sc.Send(() => state));
            });
        }

        [Fact]
        public void PostWorks()
        {
            Test(async sc =>
            {
                var state = 0;
                await sc.PostTask(5, x =>
                {
                    state = x;
                });
                Assert.Equal(5, state);
                Assert.Equal(7, await sc.PostTask(2, x => state + x));
                Assert.Equal(5, state);
                Assert.Equal(5, await sc.PostTask(() => state));
                await sc.PostTask(() =>
                {
                    state = 0;
                });
                Assert.Equal(0, state);
            });
        }

    }
}