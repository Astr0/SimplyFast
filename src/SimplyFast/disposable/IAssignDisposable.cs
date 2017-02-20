using System;

namespace SF
{
    public interface IAssignDisposable<T> : IDisposable
    {
        T Item { get; set; }
    }
}