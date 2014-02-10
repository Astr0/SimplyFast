using System.Threading.Tasks;

namespace SF.Pipes
{
    public interface IConsumer
    {
        Task<T> Take<T>();
    }
}