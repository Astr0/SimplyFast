using System.Threading.Tasks;

namespace SF.Threading
{
    public static class TaskEx
    {
        public static readonly Task Completed = Task.FromResult(true);

        public static Task<TBase> CastToBase<TDerived, TBase>(this Task<TDerived> task)
            where TDerived : TBase
        {
            var tcs = new TaskCompletionSource<TBase>();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    // ReSharper disable once PossibleNullReferenceException
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(t.Result);
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }
    }
}