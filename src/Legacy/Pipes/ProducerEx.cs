using System;
using System.Threading;
using System.Threading.Tasks;
using SF.Threading;

namespace SF.Pipes
{
    public static class ProducerEx
    {
        /// <summary>
        /// Just Select
        /// </summary>
        public static IProducer<TResult> Convert<T, TResult>(this IProducer<T> producer, Func<TResult, T> convert)
        {
            return new ConvertProducer<T,TResult>(producer, convert);
        }

        /// <summary>
        /// Just select
        /// </summary>
        public static IProducer<T> Filter<T>(this IProducer<T> producer, Func<T, bool> predicate)
        {
            // socket.ProtobufSerializer().Filter().Convert()
            return new FilterProducer<T>(producer, predicate);
        }

        public static IProducer<T> FromMethod<T>(Func<T, CancellationToken, Task> method, Action dispose = null)
        {
            return new MethodProducer<T>(method, dispose);
        }

        public static IProducer<T> FromMethod<T>(Func<T, Task> method, Action dispose = null)
        {
            return FromMethod<T>((x, ctx) => ctx.IsCancellationRequested ? TaskEx.Cancelled : method(x), dispose);
        }

        public static IProducer<T> FromMethod<T>(Action<T> method, Action dispose = null)
        {
            return FromMethod<T>((x, ctx) =>
            {
                if (ctx.IsCancellationRequested)
                    return TaskEx.Cancelled;
                method(x);
                return TaskEx.Completed;
            }, dispose);
        }
    }
}