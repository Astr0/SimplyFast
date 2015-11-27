using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Pipes
{
    internal class ConvertProducer<T, TR>: IProducer<TR>
    {
        private readonly IProducer<T> _producer;
        private readonly Func<TR, T> _convert;

        public ConvertProducer(IProducer<T> producer, Func<TR, T> convert)
        {
            _producer = producer;
            _convert = convert;
        }

        public Task Add(TR obj, CancellationToken cancellation = new CancellationToken())
        {
            return _producer.Add(_convert(obj), cancellation);
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}