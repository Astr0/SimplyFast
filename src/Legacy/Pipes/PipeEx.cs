using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Pipes
{
    public static class PipeEx
    {
        /// <summary>
        /// Consumes all events and runs action for them
        /// </summary>
        public static async Task ForEach<T>(this IConsumer<T> consumer, Action<T> action, CancellationToken cancellation = default(CancellationToken))
        {
            while (true)
            {
                T next;
                try
                {
                    next = await consumer.Take(cancellation);
                }
                catch (EndOfStreamException)
                {
                    break;
                }
                action(next);
            }
        }

        /// <summary>
        /// Consumes all events and runs async action for all of them
        /// </summary>
        public static async Task ForEach<T>(this IConsumer<T> consumer, Func<T, CancellationToken, Task> action, CancellationToken cancellation = default(CancellationToken))
        {
            while (true)
            {
                T next;
                try
                {
                    next = await consumer.Take(cancellation);
                }
                catch (EndOfStreamException)
                {
                    break;
                }
                await action(next, cancellation);
            }
        }

        /// <summary>
        /// Consumes all events and runs async action for all of them
        /// </summary>
        public static Task ForEach<T>(this IConsumer<T> consumer, Func<T, Task> action,
            CancellationToken cancellation = default(CancellationToken))
        {
            return ForEach(consumer, (obj, token) => action(obj), cancellation);
        }

        /// <summary>
        /// Sends all events from producer to consumer
        /// </summary>
        public static Task SendTo<T>(this IConsumer<T> consumer, IProducer<T> producer, CancellationToken cancellation = default (CancellationToken))
        {
            return consumer.ForEach(producer.Add, cancellation);
        }

        public static IConsumer<IDataRecord> AsPipe(this DbDataReader reader)
        {
            return new DbDataReaderConsumer(reader);
        }
    }
}