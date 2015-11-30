using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Threading
{
    public static class SynchronizationContextEx
    {
        public static void Send<T>(this SynchronizationContext context, T state, Action<T> action)
        {
            context.Send(x => action((T)x), state);
        }

        public static TResult Send<T, TResult>(this SynchronizationContext context, T state, Func<T, TResult> func)
        {
            var res = default (TResult);
            context.Send(x => res = func((T)x), state);
            return res;
        }

        public static TResult Send<TResult>(this SynchronizationContext context, Func<TResult> func)
        {
            var res = default(TResult);
            context.Send(x => res = func(), null);
            return res;
        }

        public static Task PostTask<T>(this SynchronizationContext context, T state, Action<T> action)
        {
            var tcs = new TaskCompletionSource<bool>();
            context.Post(x =>
            {
                try
                {
                    action((T) x);
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, state);
            return tcs.Task;
        }

        public static Task PostTask(this SynchronizationContext context, Action action)
        {
            var tcs = new TaskCompletionSource<bool>();
            context.Post(x =>
            {
                try
                {
                    action();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, null);
            return tcs.Task;
        }

        public static Task<TResult> PostTask<TResult>(this SynchronizationContext context, Func<TResult> func)
        {
            var tcs = new TaskCompletionSource<TResult>();
            context.Post(x =>
            {
                try
                {
                    tcs.TrySetResult(func());
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, null);
            return tcs.Task;
        }

        public static Task<TResult> PostTask<T, TResult>(this SynchronizationContext context, T state, Func<T, TResult> func)
        {
            var tcs = new TaskCompletionSource<TResult>();
            context.Post(x =>
            {
                try
                {
                    tcs.TrySetResult(func((T)x));
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, state);
            return tcs.Task;
        }
    }
}