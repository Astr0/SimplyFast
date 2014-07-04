using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Pipes
{
    /// <summary>
    /// Interface allows to consume objects like events
    /// </summary>
    public interface IConsumer<T> : IDisposable
    {
        Task<T> Take(CancellationToken cancellation = default (CancellationToken));
    }
}