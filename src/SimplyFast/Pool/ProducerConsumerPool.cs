using System.Collections.Concurrent;

namespace SF.Pool
{
    public abstract class ProducerConsumerPool<T> : IPool<T>
    {
        private readonly IProducerConsumerCollection<T> _storage;

        protected ProducerConsumerPool(IProducerConsumerCollection<T> storage = null)
        {
            _storage = storage ?? new ConcurrentBag<T>();
        }

        #region IPool<T> Members

        public T Get()
        {
            T item;
            return _storage.TryTake(out item) ? item : CreateInstance();
        }

        public void Return(T instance)
        {
            _storage.TryAdd(instance);
        }

        #endregion

        protected abstract T CreateInstance();
    }
}