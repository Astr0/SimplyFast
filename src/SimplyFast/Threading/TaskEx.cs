using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Threading
{
    /// <summary>
    /// TPL helper stuff
    /// </summary>
    public static class TaskEx
    {
        /// <summary>
        /// Just completed task
        /// </summary>
        public static readonly Task Completed = Task.FromResult(true);

        /// <summary>
        /// Casts Task`TDerived to Task`TBase
        /// </summary>
        public static Task<TBase> CastToBase<TDerived, TBase>(this Task<TDerived> task)
            where TDerived : TBase
        {
            return task.Then(x => (TBase) x);
        }

        /// <summary>
        /// Synchronously continues Task with func
        /// </summary>
        public static Task<TConvert> Then<TConvert, TSource>(this Task<TSource> task, Func<TSource, TConvert> func)
        {
            var tcs = new TaskCompletionSource<TConvert>();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    // ReSharper disable once PossibleNullReferenceException
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(func(t.Result));
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        /// <summary>
        /// Returns cancelled task or task that will be cancelled when requested by token
        /// </summary>
        public static Task<TResult> FromCancellation<TResult>(CancellationToken cancellation)
        {
            var tcs = new TaskCompletionSource<TResult>();
            if (cancellation.IsCancellationRequested)
            {
                tcs.SetCanceled();
            }
            else
            {
                cancellation.Register(tcs.SetCanceled);
            }
            return tcs.Task;
        }

        /// <summary>
        /// Returns cancelled task or task that will be cancelled when requested by token
        /// </summary>
        public static Task FromCancellation(CancellationToken cancellation)
        {
            return FromCancellation<bool>(cancellation);
        }

        /// <summary>
        /// Returns cancelled task or task that will be cancelled when requested by token. Does not cancel the actual task
        /// </summary>
        public static Task<TResult> OrCancellation<TResult>(this Task<TResult> task, CancellationToken token)
        {
            // Perf optimization for never cancelled token
            if (!token.CanBeCanceled)
                return task;

            if (task.IsCompleted)
                return task;
            var cancel = FromCancellation<TResult>(token);
            if (token.IsCancellationRequested)
                return cancel;
            // when any hack to return first - cancelled or task
            return Task.WhenAny(task, cancel).Then(x => x.Result);
        }
    }
}