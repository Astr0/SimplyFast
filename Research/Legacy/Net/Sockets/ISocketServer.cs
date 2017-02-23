using System;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Net.Sockets
{
    /// <summary>
    /// Socket server wrapper class
    /// </summary>
    public interface ISocketServer : IDisposable
    {
        Task<ISocket> Accept(CancellationToken cancellation = default (CancellationToken));
        Task Close(CancellationToken cancellation = default (CancellationToken));
    }
}