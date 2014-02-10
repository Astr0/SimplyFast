using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Pipes
{
    public static class PipeEx
    {
        public static async Task ForEach<T>(this IConsumer consumer, Action<T> action, CancellationToken cancellation)
        {
            while (true)
            {
                cancellation.ThrowIfCancellationRequested();
                T next;
                try
                {
                    next = await consumer.Take<T>();
                }
                catch (EndOfPipeException)
                {
                    break;
                }
                action(next);
            }
        }

        public static Task ForEach<T>(this IConsumer consumer, Action<T> action)
        {
            return consumer.ForEach(action, CancellationToken.None);
        }
    }
}