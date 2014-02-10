﻿using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using SF.IO;

namespace SF.Pipes
{
    public static class PipeEx
    {
        public static async Task ForEach<T>(this IConsumer<T> consumer, Action<T> action, CancellationToken cancellation)
        {
            while (true)
            {
                cancellation.ThrowIfCancellationRequested();
                T next;
                try
                {
                    next = await consumer.Take();
                }
                catch (EndOfPipeException)
                {
                    break;
                }
                action(next);
            }
        }

        public static Task ForEach<T>(this IConsumer<T> consumer, Action<T> action)
        {
            return consumer.ForEach(action, CancellationToken.None);
        }

        public static async Task ForEach<T>(this IConsumer<T> consumer, Func<T, Task> action, CancellationToken cancellation)
        {
            while (true)
            {
                cancellation.ThrowIfCancellationRequested();
                T next;
                try
                {
                    next = await consumer.Take();
                }
                catch (EndOfPipeException)
                {
                    break;
                }
                await action(next);
            }
        }

        public static Task ForEach<T>(this IConsumer<T> consumer, Func<T, Task> action)
        {
            return consumer.ForEach(action, CancellationToken.None);
        }

        public static Task SendTo<T>(this IConsumer<T> consumer, IProducer<T> producer, CancellationToken cancellation)
        {
            return consumer.ForEach((Func<T, Task>) producer.Add, cancellation);
        }

        public static Task SendTo<T>(this IConsumer<T> consumer, IProducer<T> producer)
        {
            return consumer.SendTo(producer, CancellationToken.None);
        }

        public static IConsumer<IDataRecord> AsPipe(this DbDataReader reader)
        {
            return new DbDataReaderPipe(reader);
        }

        /// <summary>
        ///     No framing, just sends bytes to stream
        /// </summary>
        public static IProducer<ArraySegment<byte>> AsBinaryPipe(this IOutputStream stream)
        {
            return new OutputStreamPipe(stream);
        }

        /// <summary>
        ///     No framing, just reads bytes from stream
        /// </summary>
        public static IConsumer<ArraySegment<byte>> AsBinaryPipe(this IInputStream stream, ArraySegment<byte> buffer)
        {
            return new InputStreamPipe(stream, buffer);
        }

        /// <summary>
        ///     No framing, just reads bytes from stream
        /// </summary>
        public static IConsumer<ArraySegment<byte>> AsBinaryPipe(this IInputStream stream, int bufferSize = 256)
        {
            return new InputStreamPipe(stream, new ArraySegment<byte>(new byte[bufferSize]));
        }
    }
}