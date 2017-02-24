using System;

namespace SimplyFast.Pool
{
    public interface IPooled<out T>: IDisposable
    {
        T Instance { get; }
    }
}