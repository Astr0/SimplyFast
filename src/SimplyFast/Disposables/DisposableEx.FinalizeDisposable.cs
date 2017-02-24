using System;
using System.Threading;

namespace SimplyFast.Disposables
{
    public static partial class DisposableEx
    {
        private class FinalizeDisposable : IDisposable
        {
            public FinalizeDisposable(IDisposable underlying)
            {
                _underlying = underlying;
            }

            ~FinalizeDisposable()
            {
                Dispose();
            }

            private readonly IDisposable _underlying;
            private int _disposed;

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 1)
                    return;
                _underlying.Dispose();
            }
        }
    }
}