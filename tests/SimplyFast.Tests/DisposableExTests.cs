using System;
using NUnit.Framework;

namespace SF.Tests
{
    [TestFixture]
    public class DisposableExTests
    {
        [Test]
        public void KeepAliveTest()
        {
            WeakReference wr;
            {
                wr = new WeakReference(new object());
                using (DisposableEx.Null().KeepAlive(wr.Target))
                {
                    GC.Collect();
                    GC.WaitForFullGCComplete();
                    Assert.IsTrue(wr.IsAlive);
                }
            }
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void ActionDoesNotAcceptNullAction()
        {
            Assert.Throws<ArgumentNullException>(() => DisposableEx.Action(null));
        }

        [Test]
        public void ActionInvokesOnDispose()
        {
            var a = 1;
            using (DisposableEx.Action(() => a = 3))
            {
                a = 2;
            }
            Assert.AreEqual(3, a);
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
    }
}