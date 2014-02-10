using System;
using System.Threading.Tasks;

namespace SF.Pipes
{
    public interface IProducer<in T>: IDisposable
    {
        Task Add(T obj);
    }
}