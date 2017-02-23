using System;

namespace SF.Pool
{
    public interface IPooled<out T>: IDisposable
    {
        T Instance { get; }
    }
}