using System;
using System.IO;
using System.Threading.Tasks;

namespace SF.Net.Sockets
{
    public interface ISocket: IDisposable
    {
        Stream Stream { get; }
        Task Disconnect();
    }
}