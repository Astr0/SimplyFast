namespace SF.Pool.Internal
{
    internal class NotPooledWrapper<T> : IPooled<T>
    {
        private readonly T _instance;
        private volatile int _disposed;

        public NotPooledWrapper(T instance)
        {
            _instance = instance;
        }

        public void Dispose()
        {
            _disposed = 1;
        }

        public T Instance
        {
            get
            {
                if (_disposed == 1)
                    throw PooledEx.InPoolException();
                return _instance;
            }
        }
    }
}