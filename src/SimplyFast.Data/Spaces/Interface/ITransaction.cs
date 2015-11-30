using System;
using System.Threading.Tasks;

namespace SF.Data.Spaces
{
    public interface ITransaction : IDisposable
    {
        Task Abort();
        Task Commit();
    }
}