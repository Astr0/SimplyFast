using System.Collections.Concurrent;

namespace SF.Pool
{
    /// <summary>
    /// Abstract pool that uses IProducerConsumerCollection to store objects
    /// </summary>
    public abstract class ProducerConsumerPool<T> : IPool<T>
    {
        private readonly IProducerConsumerCollection<T> _storage;

        protected ProducerConsumerPool(IProducerConsumerCollection<T> storage = null)
        {
            _storage = storage ?? new ConcurrentBag<T>();
        }

        #region IPool<T> Members

        public virtual T Get()
        {
            T item;
            if (!_storage.TryTake(out item)) 
                return CreateInstance();
            Prepare(item);
            return item;
        }

        public virtual bool Return(T instance)
        {
            return _storage.TryAdd(instance);
        }

        #endregion

        protected virtual void Prepare(T item)
        {
            
        }

        
        protected abstract T CreateInstance();
    }
}