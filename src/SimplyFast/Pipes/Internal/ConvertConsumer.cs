using System;
using System.Threading;
using System.Threading.Tasks;
using SF.Threading;

namespace SF.Pipes
{
    internal class ConvertConsumer<T, TConvert>: IConsumer<TConvert>
    {
        private readonly IConsumer<T> _consumer;
        private readonly Func<Task<T>, Task<TConvert>> _convert;

        public ConvertConsumer(IConsumer<T> consumer, Func<Task<T>, Task<TConvert>> convert)
        {
            _consumer = consumer;
            _convert = convert;
        }

        public Task<TConvert> Take(CancellationToken cancellation = new CancellationToken())
        {
            return _convert(_consumer.Take(cancellation));
        }

        public void Dispose()
        {
            _consumer.Dispose();
        }
    }
}