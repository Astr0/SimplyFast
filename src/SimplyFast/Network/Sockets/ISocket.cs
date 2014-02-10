using System.Threading.Tasks;
using SF.IO;

namespace SF.Network.Sockets
{
    public interface ISocket: IInputStream, IOutputStream
    {
        Task Disconnect();
    }
}