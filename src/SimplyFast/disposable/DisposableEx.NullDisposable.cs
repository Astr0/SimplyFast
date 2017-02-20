using System;

namespace SF
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