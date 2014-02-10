using System;
using System.Threading.Tasks;

namespace SF.Pipes
{
    public interface IConsumer<T>: IDisposable
    {
        Task<T> Take();
    }
}