using System;

namespace SimplyFast.Disposables
{
    public static partial class DisposableEx
    {
        private class NullDisposable : IDisposable
        {
            public static readonly NullDisposable Instance = new NullDisposable();

            private NullDisposable()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}