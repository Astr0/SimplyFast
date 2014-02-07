using System.Threading.Tasks;
using SF.IO;

namespace SF.Network.Sockets
{
    public interface ISocket: IDuplexStream
    {
        Task Disconnect();
    }
}