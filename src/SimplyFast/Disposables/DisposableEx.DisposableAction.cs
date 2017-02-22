using System;
using System.Threading;

namespace SF.Disposables
{
    public static partial class DisposableEx
    {
        private sealed class DisposableAction : IDisposable
        {
            private Action _action;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                var act = Interlocked.Exchange(ref _action, null);
                act?.Invoke();
            }
        }
    }
}