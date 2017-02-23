using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Pipes
{
    internal class MethodConsumer<T>: IConsumer<T>
    {
        private readonly Func<CancellationToken, Task<T>> _method;
        private readonly Action _dispose;

        public MethodConsumer(Func<CancellationToken, Task<T>> method, Action dispose)
        {
            _method = method;
            _dispose = dispose;
        }

        public Task<T> Take(CancellationToken cancellation = new CancellationToken())
        {
            return _method(cancellation);
        }

        public void Dispose()
        {
            if (_dispose != null)
                _dispose();
        }
    }
}