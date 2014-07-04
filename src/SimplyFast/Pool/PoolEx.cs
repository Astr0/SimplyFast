using System;

namespace SF.Pool
{
    /// <summary>
    /// Pool utils
    /// </summary>
    public static class PoolEx
    {
        /// <summary>
        /// No pooling, always new T()
        /// </summary>
        public static IPool<T> None<T>() where T : new()
        {
            return new NullPool<T>(() => new T());
        }
        /// <summary>
        /// No pooling, always activator()
        /// </summary>
        public static IPool<T> None<T>(Func<T> activator)
        {
            return new NullPool<T>(activator);
        }

        /// <summary>
        /// Basic pooling using new T()
        /// </summary>
        public static IPool<T> Basic<T>() where T : new()
        {
            return new ActivatorPool<T>(() => new T());
        }

        /// <summary>
        /// Basic pooling using activator()
        /// </summary>
        public static IPool<T> Basic<T>(Func<T> activator)
        {
            return new ActivatorPool<T>(activator);
        }
    }
}