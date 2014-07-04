using System;

namespace SF.IoC
{
    /// <summary>
    /// Service locator facade. Static for convenience
    /// </summary>
    public static class ServiceLocator
    {
        private static IServiceLocator _locator;

        /// <summary>
        /// Set ServiceLocator implementation
        /// </summary>
        public static void SetLocator(IServiceLocator locator)
        {
            _locator = locator;
        }

        /// <summary>
        /// Get Service strong typed
        /// </summary>
        public static T Get<T>()
        {
            var loc = _locator;
            if (loc == null)
                throw NoServiceLocator();
            return loc.Get<T>();
        }

        /// <summary>
        /// Get Service loose typed
        /// </summary>
        public static object Get(Type serviceType)
        {
            var loc = _locator;
            if (loc == null)
                throw NoServiceLocator();
            return loc.Get(serviceType);
        }

        private static InvalidOperationException NoServiceLocator()
        {
            return new InvalidOperationException("No service locator was set");
        }
    }
}