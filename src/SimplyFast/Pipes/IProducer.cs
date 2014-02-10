using System.Threading.Tasks;

namespace SF.Pipes
{
    public interface IProducer
    {
        Task Add<T>(T obj);
    }
}