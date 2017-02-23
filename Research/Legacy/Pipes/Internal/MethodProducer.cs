using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Pipes
{
    internal class MethodProducer<T> : IProducer<T>
    {
        private readonly Func<T, CancellationToken, Task> _method;
        private readonly Action _dispose;

        public MethodProducer(Func<T, CancellationToken, Task> method, Action dispose)
        {
            _method = method;
            _dispose = dispose;
        }


        public Task Add(T obj, CancellationToken cancellation = new CancellationToken())
        {
            return _method(obj, cancellation);
        }

        public void Dispose()
        {
            if (_dispose != null)
                _dispose();
        }
    }
}