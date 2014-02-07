using System;
using System.Threading.Tasks;

namespace SF.Network.Sockets
{
    public interface ISocketServer: IDisposable
    {
        Task<ISocket> Accept();
        Task Close();
    }
}