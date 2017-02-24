using System;

namespace SimplyFast.Disposables
{
    public interface IAssignDisposable<T> : IDisposable
    {
        T Item { get; set; }
    }
}