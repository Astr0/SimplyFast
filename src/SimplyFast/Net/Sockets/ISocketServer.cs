using System;
using System.Threading.Tasks;

namespace SF.Net.Sockets
{
    public interface ISocketServer: IDisposable
    {
        Task<ISocket> Accept();
        Task Close();
    }
}