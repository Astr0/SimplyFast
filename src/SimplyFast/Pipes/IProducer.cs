using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Pipes
{
    /// <summary>
    /// Interface allows to produce objects like events
    /// </summary>
    public interface IProducer<in T> : IDisposable
    {
        Task Add(T obj, CancellationToken cancellation = default (CancellationToken));
    }
}