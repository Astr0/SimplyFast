using System;
using System.Threading;
using System.Threading.Tasks;
using SF.Threading;

namespace SF.Pipes
{
    internal class FilterProducer<T> : IProducer<T>
    {
        private readonly IProducer<T> _producer;
        private readonly Func<T, bool> _predicate;

        public FilterProducer(IProducer<T> producer, Func<T, bool> predicate)
        {
            _producer = producer;
            _predicate = predicate;
        }

        public Task Add(T obj, CancellationToken cancellation = new CancellationToken())
        {
            if (!_predicate(obj))
            {
                return cancellation.IsCancellationRequested ? TaskEx.Cancelled : TaskEx.Completed;
            }
            return _producer.Add(obj, cancellation);
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}