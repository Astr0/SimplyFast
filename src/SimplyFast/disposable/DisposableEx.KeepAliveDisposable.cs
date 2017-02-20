using System;
using System.Diagnostics.CodeAnalysis;

namespace SF
{
    public static partial class DisposableEx
    {
        private class KeepAliveDisposable : IDisposable
        {
            private readonly IDisposable _underlying;
            [SuppressMessage("ReSharper", "NotAccessedField.Local")]
            private object _keepAliveObject;

            public KeepAliveDisposable(IDisposable underlying, object keepAliveObject)
            {
                _underlying = underlying;
                _keepAliveObject = keepAliveObject;
            }

            public void Dispose()
            {
                _keepAliveObject = null;
                _underlying.Dispose();
            }
        }
    }
}