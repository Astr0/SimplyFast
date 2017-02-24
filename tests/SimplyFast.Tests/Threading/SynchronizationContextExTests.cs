using System;
using System.Threading;
using NUnit.Framework;
using SimplyFast.Threading;

namespace SimplyFast.Tests.Threading
{
    [TestFixture]
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
            Assert.IsTrue(done);
        }

        [Test]
        public void SendWorks()
        {
            Test(sc =>
            {
                var state = 0;
                sc.Send(5, x =>
                {
                    state = x;
                });
                Assert.AreEqual(5, state);
                Assert.AreEqual(7, sc.Send(2, x => state + x));
                Assert.AreEqual(5, state);
                Assert.AreEqual(5, sc.Send(() => state));
            });
        }

        [Test]
        public void PostWorks()
        {
            Test(async sc =>
            {
                var state = 0;
                await sc.PostTask(5, x =>
                {
                    state = x;
                });
                Assert.AreEqual(5, state);
                Assert.AreEqual(7, await sc.PostTask(2, x => state + x));
                Assert.AreEqual(5, state);
                Assert.AreEqual(5, await sc.PostTask(() => state));
                await sc.PostTask(() =>
                {
                    state = 0;
                });
                Assert.AreEqual(0, state);
            });
        }

    }
}