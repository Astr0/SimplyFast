using System;
using SimplyFast.Pool.Internal;
using System.Collections.Concurrent;

namespace SimplyFast.Pool
{
    /// <summary>
    /// Pool utils
    /// </summary>
    public static class PoolEx
    {
        /// <summary>
        /// No pooling, always calls factory and no return
        /// </summary>
        public static IPool<T, TParam> None<T, TParam>(InitPooled<T, TParam> init, ReturnToPoll<T> done = null) 
        {
            return new NullPool<T, TParam>(init, done);
        }
       
        public static IPool<T, TParam> ThreadSafe<T, TParam>(InitPooled<T, TParam> init, ReturnToPoll<T> done = null)
        {
            return Concurrent(init, done);
        }

        public static IPool<T, TParam> Locking<T, TParam>(this IPool<T, TParam> pool)
        {
            return new LockingPool<T, TParam>(pool);
        }

        public static IPool<T, TParam> ThreadUnsafe<T, TParam>(InitPooled<T, TParam> init, ReturnToPoll<T> done = null)
        {
            return new StackPool<T, TParam>(init, done);
        }

        /// <summary>
        /// Basic pooling using underlying producer consumer collection
        /// </summary>
        public static IPool<T, TParam> Concurrent<T, TParam>(InitPooled<T, TParam> init, ReturnToPoll<T> done = null, IProducerConsumerCollection<T> storage = null)
        {
            return new ProducerConsumerPool<T, TParam>(init, done, storage);
        }
    }
}