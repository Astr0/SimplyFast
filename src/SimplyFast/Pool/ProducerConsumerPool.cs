using System.Collections.Concurrent;

namespace SF.Pool
{
    public abstract class ProducerConsumerPool<T>: IPool<T>
    {
        private readonly IProducerConsumerCollection<T> _storage;

        protected ProducerConsumerPool(IProducerConsumerCollection<T> storage = null)
        {
            _storage = storage ?? new ConcurrentBag<T>();
        }

        protected abstract T CreateInstance();

        public T Get()
        {
            T item;
            return _storage.TryTake(out item) ? item : CreateInstance();
        }

        public void Return(T instance)
        {
            _storage.TryAdd(instance);
        }
    }
}