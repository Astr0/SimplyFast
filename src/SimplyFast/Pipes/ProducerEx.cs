using System;

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
    }
}