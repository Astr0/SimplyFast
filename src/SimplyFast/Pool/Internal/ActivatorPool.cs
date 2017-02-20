using System;
using System.Collections.Concurrent;

namespace SF.Pool
{
    /// <summary>
    /// Basic pool that uses IProducerConsumerCollection to store pooled objects and can create instances using constructor
    /// </summary>
    internal class ActivatorPool<T> : ProducerConsumerPool<T>
    {
        private readonly Func<T> _activator;

        public ActivatorPool(Func<T> activator, IProducerConsumerCollection<T> storage = null) : base(storage ?? new ConcurrentBag<T>())
        {
            if (activator == null)
                throw new ArgumentNullException(nameof(activator));
            _activator = activator;
        }

        protected override T CreateInstance()
        {
            return _activator();
        }
    }
}