using System;
using System.Threading.Tasks;
using SF.Threading;

namespace SF.Pipes
{
    public static class ConsumerEx
    {
        /// <summary>
        /// Casts consumer of derived class to base class
        /// </summary>
        public static IConsumer<TConvert> Convert<T, TConvert>(this IConsumer<T> consumer, Func<Task<T>, Task<TConvert>> convert)
        {
            return new ConvertConsumer<T, TConvert>(consumer, convert);
        }

        /// <summary>
        /// Casts consumer of derived class to base class
        /// </summary>
        public static IConsumer<TBase> CastToBase<TDerived, TBase>(this IConsumer<TDerived> consumer)
            where TDerived: TBase
        {
            return consumer.Convert(TaskEx.CastToBase<TDerived, TBase>);
        }

        /// <summary>
        /// Just Select
        /// </summary>
        public static IConsumer<TResult> Select<T, TResult>(this IConsumer<T> consumer, Func<T, TResult> selector)
        {
            return consumer.Convert(x => x.Then(selector));
        }

        /// <summary>
        /// Just select
        /// </summary>
        public static IConsumer<T> Where<T>(this IConsumer<T> consumer, Func<T, bool> predicate)
        {
            return new WhereConsumer<T>(consumer, predicate);
        }

        ///// <summary>
        ///// Creates consumer from event pattern. Action will block if nothing taken
        ///// </summary>
        //public static IConsumer<T> FromEvent<T>(Func<Action<T>, IDisposable> subscribe)
        //{
        //    return new EventConsumer<T>(subscribe);
        //}
    }
}