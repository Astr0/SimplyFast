using System;

namespace SF.Disposables
{
    public interface IAssignDisposable<T> : IDisposable
    {
        T Item { get; set; }
    }
}