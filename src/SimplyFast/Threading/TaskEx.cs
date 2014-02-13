using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Threading
{
    public static class TaskEx
    {
        public static readonly Task Completed = Task.FromResult(true);

        public static Task<TBase> CastToBase<TDerived, TBase>(this Task<TDerived> task)
            where TDerived : TBase
        {
            return task.Convert(x => (TBase) x);
        }

        public static Task<TConvert> Convert<TConvert, TSource>(this Task<TSource> task, Func<TSource, TConvert> conversion)
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
                    tcs.TrySetResult(conversion(t.Result));
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public static Task<TResult> FromCancellation<TResult>(CancellationToken cancellation)
        {
            if (!cancellation.IsCancellationRequested)
                throw new ArgumentOutOfRangeException("cancellation");
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetCanceled();
            return tcs.Task;
        }
    }
}