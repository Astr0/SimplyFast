using System.Collections.Concurrent;

namespace SF.Pool
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
       
        /// <summary>
        /// Basic pooling using underlying producer consumer collection
        /// </summary>
        public static IPool<T> Basic<T>(PooledFactory<T> factory, IProducerConsumerCollection<T> storage = null)
        {
            return new ProducerConsumerPool<T>(factory, storage);
        }
    }
}