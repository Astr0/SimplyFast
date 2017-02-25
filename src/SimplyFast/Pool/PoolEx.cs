using SimplyFast.Pool.Internal;
#if CONCURRENT
using System.Collections.Concurrent;
#endif

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
        public static IPool<T> None<T>(PooledFactory<T> factory) 
        {
            return new NullPool<T>(factory);
        }
       
        public static IPool<T> ThreadSafe<T>(PooledFactory<T> factory)
        {
#if CONCURRENT
            return Concurrent(factory, new ConcurrentBag<T>());
#else
            return ThreadSafeLocking(factory);
#endif
        }

        public static IPool<T> ThreadSafeLocking<T>(PooledFactory<T> factory)
        {
            return new StackLockPool<T>(factory);
        }

        public static IPool<T> ThreadUnsafe<T>(PooledFactory<T> factory)
        {
            return new StackPool<T>(factory);
        }

#if CONCURRENT
        /// <summary>
        /// Basic pooling using underlying producer consumer collection
        /// </summary>
        public static IPool<T> Concurrent<T>(PooledFactory<T> factory, IProducerConsumerCollection<T> storage)
        {
            return new ProducerConsumerPool<T>(factory, storage);
        }

        /// <summary>
        /// Basic pooling using underlying producer consumer collection
        /// </summary>
        public static IPool<T> Concurrent<T>(PooledFactory<T> factory)
        {
            return Concurrent(factory, new ConcurrentBag<T>());
        }
#endif
    }
}