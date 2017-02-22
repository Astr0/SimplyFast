using System;
using System.Threading;

namespace SF.Disposables
{
    public static partial class DisposableEx
    {
        private class ContextDisposable : IDisposable
        {
            private readonly IDisposable _disposable;
            private readonly SynchronizationContext _context;


            public ContextDisposable(IDisposable disposable, SynchronizationContext context)
            {
                _disposable = disposable;
                _context = context;
            }

            public void Dispose()
            {
                _context.Send(x => ((IDisposable)x).Dispose(), _disposable);
            }
        }
    }
}