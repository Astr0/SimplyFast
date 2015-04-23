using System;

namespace SF.Pool
{
    /// <summary>
    /// Pool implementation that always creates new instances and destroys returned
    /// </summary>
    internal class NullPool<T> : IPool<T>
    {
        private readonly Func<T> _activator;

        public NullPool(Func<T> activator)
        {
            if (activator == null)
                throw new ArgumentNullException("activator");
            _activator = activator;
        }

        #region IPool<T> Members

        public T Get()
        {
            return _activator();
        }

        public bool Return(T instance)
        {
            var disposabe = instance as IDisposable;
            if (disposabe != null)
                disposabe.Dispose();
            return true;
        }

        #endregion
    }
}