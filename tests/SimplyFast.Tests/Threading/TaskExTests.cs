using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SimplyFast.Threading;

namespace SimplyFast.Tests.Threading
{
    [TestFixture]
    public class TaskExTests
    {
        [Test]
        public void CompletedCompleted()
        {
            var t = TaskEx.Completed;
            Assert.IsTrue(t.IsCompleted);
            Assert.IsFalse(t.IsCanceled);
            Assert.IsFalse(t.IsFaulted);
        }

        private static async Task<int> Throw(int milliseconds)
        {
            if (milliseconds == 0)
                throw new InvalidOperationException("ex");
            await Task.Delay(milliseconds);
            throw new InvalidOperationException("ex");
        }

        [Test]
        public void CastToBaseWorks()
        {
            Assert.AreEqual(5, Task.FromResult(5).CastToBase<int, object>().Result);
        }

        [Test]
        public void ThenWorks()
        {
            Assert.AreEqual(2.5, Task.FromResult(5).Then(x => x / 2.0).Result);
            
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await Throw(0).Then(x => x/2.0);
            });

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await Throw(1).Then(x => x / 2.0);
            });

            Assert.IsTrue(TaskEx.FromCancellation<int>(new CancellationToken(true)).Then(x => x / 2.0).IsCanceled);
        }

        [Test]
        public void FromCancellationWorks()
        {
            Assert.IsFalse(TaskEx.FromCancellation(CancellationToken.None).IsCanceled);
            Assert.IsTrue(TaskEx.FromCancellation(new CancellationToken(true)).IsCanceled);
            using (var cts = new CancellationTokenSource())
            {
                var task = TaskEx.FromCancellation<int>(cts.Token);
                Assert.IsFalse(task.IsCanceled);
                cts.Cancel();
                Assert.IsTrue(task.IsCanceled);
            }
        }


        [Test]
        public void OrCancellationWorks()
        {
            var comp = Task.FromResult(true);
            Assert.AreEqual(comp, comp.OrCancellation(new CancellationToken(true)));
            Assert.AreEqual(comp, comp.OrCancellation(new CancellationToken(false)));
            var never = new TaskCompletionSource<int>().Task;
            Assert.IsTrue(never.OrCancellation(new CancellationToken(true)).IsCanceled);
            using (var cts = new CancellationTokenSource())
            {
                var task = never.OrCancellation(cts.Token);
                Assert.IsFalse(task.IsCanceled);
                cts.Cancel();
                Assert.IsTrue(task.IsCanceled);
            }
        }
    }
}