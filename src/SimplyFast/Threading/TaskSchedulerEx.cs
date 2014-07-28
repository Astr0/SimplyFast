using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Threading
{
    public static class TaskSchedulerEx
    {
        public static Task Start(this TaskScheduler scheduler, Action action, CancellationToken cancellation = new CancellationToken(), TaskCreationOptions options = TaskCreationOptions.None)
        {
            var task = new Task(action, cancellation, options);
            task.Start(scheduler);
            return task;
        }

        public static Task Start(this TaskScheduler scheduler, Action<object> action, object state,
            CancellationToken cancellation = new CancellationToken(), TaskCreationOptions options = TaskCreationOptions.None)
        {
            var task = new Task(action, state, cancellation, options);
            task.Start(scheduler);
            return task;
        }

        public static Task<T> Start<T>(this TaskScheduler scheduler, Func<T> function, CancellationToken cancellation = new CancellationToken(), TaskCreationOptions options = TaskCreationOptions.None)
        {
            var task = new Task<T>(function, cancellation, options);
            task.Start(scheduler);
            return task;
        }

        public static Task<T> Start<T>(this TaskScheduler scheduler, Func<object, T> function, object state, CancellationToken cancellation = new CancellationToken(), TaskCreationOptions options = TaskCreationOptions.None)
        {
            var task = new Task<T>(function, state, cancellation, options);
            task.Start(scheduler);
            return task;
        }
    }
}