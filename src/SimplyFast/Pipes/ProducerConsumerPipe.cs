using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Pipes
{
    public class ProducerConsumerPipe<T> : IProducer<T>, IConsumer<T>
    {
        private readonly IProducerConsumerCollection<T> _storage;
        private volatile TaskCompletionSource<Task> _added = new TaskCompletionSource<Task>();
        private volatile TaskCompletionSource<Task> _taken = new TaskCompletionSource<Task>();

        public ProducerConsumerPipe(IProducerConsumerCollection<T> storage = null)
        {
            _storage = storage ?? new ConcurrentQueue<T>();
        }

        #region IConsumer<T> Members

        public async Task<T> Take()
        {
            var added = _added.Task;
            T obj;
            while (!_storage.TryTake(out obj))
            {
                added = (Task<Task>) (await added);
            }
            var newTaken = new TaskCompletionSource<Task>();
            // ReSharper disable once CSharpWarnings::CS0420
            var currentTaken = Interlocked.Exchange(ref _taken, newTaken);
            currentTaken.SetResult(newTaken.Task);
            return obj;
        }

        #endregion

        #region IProducer<T> Members

        public async Task Add(T obj)
        {
            var taken = _taken.Task;
            while (!_storage.TryAdd(obj))
            {
                taken = (Task<Task>) await taken;
            }
            var newAdded = new TaskCompletionSource<Task>();
            // ReSharper disable once CSharpWarnings::CS0420
            var currentAdded = Interlocked.Exchange(ref _added, newAdded);
            currentAdded.SetResult(newAdded.Task);
        }

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}