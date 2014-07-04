using System;

namespace SF.IoC
{
    /// <summary>
    /// Service locator facade. Static for convenience
    /// </summary>
    public static class ServiceLocator
    {
        private static volatile Func<Type, object> _locator;

        /// <summary>
        /// Set ServiceLocator implementation
        /// </summary>
        public static void SetLocator(Func<Type, object> locator)
        {
            _locator = locator;
        }

        /// <summary>
        /// Get Service strong typed
        /// </summary>
        public static T Get<T>()
        {
            return (T) Get(typeof (T));
        }

        /// <summary>
        /// Get Service loose typed
        /// </summary>
        public static object Get(Type serviceType)
        {
            var loc = _locator;
            return loc != null ? loc(serviceType) : null;
        }
    }
}