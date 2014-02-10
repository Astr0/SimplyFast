using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using SF.Threading;

namespace SF.Pipes
{
    public class ProducerConsumerPipe<TStorage> : IProducer, IConsumer
    {
        private readonly IProducerConsumerCollection<TStorage> _storage;
        private volatile TaskCompletionSource<Task> _added = new TaskCompletionSource<Task>();
        private volatile TaskCompletionSource<Task> _taken = new TaskCompletionSource<Task>();

        public ProducerConsumerPipe(IProducerConsumerCollection<TStorage> storage = null)
        {
            _storage = storage ?? new ConcurrentQueue<TStorage>();
        }

        #region IConsumer Members

        Task<T> IConsumer.Take<T>()
        {
            return Take().Convert(x => (T) (object) x);
        }

        #endregion

        #region IProducer Members

        Task IProducer.Add<T>(T obj)
        {
            return Add((TStorage) (object) obj);
        }

        #endregion

        public async Task<TStorage> Take()
        {
            var added = _added.Task;
            TStorage obj;
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

        public async Task Add(TStorage obj)
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
    }
}