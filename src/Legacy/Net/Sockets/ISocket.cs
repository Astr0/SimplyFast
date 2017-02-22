using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Net.Sockets
{
    /// <summary>
    /// Socket wrapper class
    /// </summary>
    public interface ISocket : IDisposable
    {
        Stream Stream { get; }
        Task Disconnect(CancellationToken cancellation = default (CancellationToken));
    }
}