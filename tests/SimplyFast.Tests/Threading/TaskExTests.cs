using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using SimplyFast.Threading;

namespace SimplyFast.Tests.Threading
{
    
    public class TaskExTests
    {
        [Fact]
        public void CompletedCompleted()
        {
            var t = TaskEx.Completed;
            Assert.True(t.IsCompleted);
            Assert.False(t.IsCanceled);
            Assert.False(t.IsFaulted);
        }

        private static async Task<int> Throw(int milliseconds)
        {
            if (milliseconds == 0)
                throw new InvalidOperationException("ex");
            await Task.Delay(milliseconds);
            throw new InvalidOperationException("ex");
        }

        [Fact]
        public void CastToBaseWorks()
        {
            Assert.Equal(5, Task.FromResult(5).CastToBase<int, object>().Result);
        }

        [Fact]
        public void ThenWorks()
        {
            Assert.Equal(2.5, Task.FromResult(5).Then(x => x / 2.0).Result);
            
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await Throw(0).Then(x => x/2.0);
            });

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await Throw(1).Then(x => x / 2.0);
            });

            Assert.True(TaskEx.FromCancellation<int>(new CancellationToken(true)).Then(x => x / 2.0).IsCanceled);
        }

        [Fact]
        public void FromCancellationWorks()
        {
            Assert.False(TaskEx.FromCancellation(CancellationToken.None).IsCanceled);
            Assert.True(TaskEx.FromCancellation(new CancellationToken(true)).IsCanceled);
            using (var cts = new CancellationTokenSource())
            {
                var task = TaskEx.FromCancellation<int>(cts.Token);
                Assert.False(task.IsCanceled);
                cts.Cancel();
                Assert.True(task.IsCanceled);
            }
        }

        [Fact]
        public void OrCancellationWorks()
        {
            var comp = Task.FromResult(true);
            Assert.Equal(comp, comp.OrCancellation(new CancellationToken(true)));
            Assert.Equal(comp, comp.OrCancellation(new CancellationToken(false)));
            var never = new TaskCompletionSource<int>().Task;
            Assert.True(never.OrCancellation(new CancellationToken(true)).IsCanceled);
            using (var cts = new CancellationTokenSource())
            {
                var task = never.OrCancellation(cts.Token);
                Assert.False(task.IsCanceled);
                cts.Cancel();
                Assert.True(task.IsCanceled);
            }
        }
    }
}