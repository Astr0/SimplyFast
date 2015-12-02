using System;
using System.Threading.Tasks;

namespace SF.Data.Legacy.Spaces
{
    public interface ISpaceTable<T> : ISyncSpaceTable<T> where T : class
    {
        Task<T> Read(IQuery<T> query, TimeSpan timeout, ITransaction transaction = null);
        Task<T> TryRead(IQuery<T> query, ITransaction transaction = null);
        Task<T> Take(IQuery<T> query, TimeSpan timeout, ITransaction transaction = null);
        Task<T> TryTake(IQuery<T> query, ITransaction transaction = null);
        Task<T[]> Scan(IQuery<T> query, ITransaction transaction = null);
        Task<int> Count(IQuery<T> query, ITransaction transaction = null);
        Task Add(T tuple, ITransaction transaction = null);
        Task AddRange(T[] tuples, ITransaction transaction = null);
    }
}