using System;

namespace SF.IoC
{
    /// <summary>
    /// Service locator implementation
    /// </summary>
    public interface IServiceLocator
    {
        /// <summary>
        /// Strong typed Get service
        /// </summary>
        T Get<T>();
        /// <summary>
        /// Loosely typed Get service
        /// </summary>
        object Get(Type type);
    }
}