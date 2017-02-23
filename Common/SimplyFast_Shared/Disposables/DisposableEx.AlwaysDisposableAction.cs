using System;

namespace SF.Disposables
{
    public static partial class DisposableEx
    {
        private class AlwaysDisposableAction : IDisposable
        {
            private readonly Action _action;

            public AlwaysDisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }
    }
}