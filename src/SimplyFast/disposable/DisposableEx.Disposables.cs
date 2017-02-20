using System;

namespace SF
{
    public static partial class DisposableEx
    {
        private sealed class Disposables : IDisposable
        {
            private readonly IDisposable[] _disposables;

            public Disposables(params IDisposable[] disposables)
            {
                _disposables = disposables;
            }

            #region IDisposable Members

            public void Dispose()
            {
                if (_disposables == null)
                    return;
                foreach (var disposable in _disposables)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    // ReSharper disable EmptyGeneralCatchClause
                    catch
                    // ReSharper restore EmptyGeneralCatchClause
                    {
                    }
                }
            }

            #endregion
        }
    }
}