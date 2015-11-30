using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Data.Spaces
{
    public interface ISpace<T> : IDisposable
        where T: class
    {
        Task<ITransaction> BeginTransaction();
        Task<T> Read(IQuery<T> query, CancellationToken token, ITransaction transaction = null);
        Task<T> TryRead(IQuery<T> query, ITransaction transaction = null);
        Task<T> Take(IQuery<T> query, CancellationToken token, ITransaction transaction = null);
        Task<T> TryTake(IQuery<T> query, ITransaction transaction = null);
        Task<T[]> Scan(IQuery<T> query, ITransaction transaction = null);
        Task<int> Count(IQuery<T> query, ITransaction transaction = null);
        Task Add(T tuple, ITransaction transaction = null);
        Task AddRange(IEnumerable<T> tuples, ITransaction transaction = null);
    }
}