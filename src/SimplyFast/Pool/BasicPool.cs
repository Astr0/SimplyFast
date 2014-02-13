using System.Collections.Concurrent;

namespace SF.Pool
{
    public class BasicPool<T> : ProducerConsumerPool<T>
        where T : new()
    {
        public BasicPool(IProducerConsumerCollection<T> storage = null) : base(storage ?? new ConcurrentBag<T>())
        {
        }

        protected override T CreateInstance()
        {
            return new T();
        }
    }
}