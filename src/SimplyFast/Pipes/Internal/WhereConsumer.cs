using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Pipes
{
    internal class WhereConsumer<T>: IConsumer<T>
    {
        private readonly IConsumer<T> _consumer;
        private readonly Func<T, bool> _predicate;

        public WhereConsumer(IConsumer<T> consumer, Func<T, bool> predicate)
        {
            _consumer = consumer;
            _predicate = predicate;
        }

        public async Task<T> Take(CancellationToken cancellation = new CancellationToken())
        {
            while (true)
            {
                var result = await _consumer.Take(cancellation);
                if (_predicate(result))
                    return result;
            }
        }

        public void Dispose()
        {
            _consumer.Dispose();
        }
    }
}