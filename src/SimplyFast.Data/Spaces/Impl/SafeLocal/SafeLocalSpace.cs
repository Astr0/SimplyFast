using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Data.Spaces
{
    internal class SafeLocalSpace<T> : ISpace<T> where T : class
    {
        private readonly ILocalSpace<T> _space;
        private readonly Func<T, T> _clone;
        private readonly SynchronizationContext _context;

        public SafeLocalSpace(ILocalSpace<T> space, Func<T, T> clone, SynchronizationContext context = null)
        {
            _space = space;
            _clone = clone;
            _context = context ?? SynchronizationContext.Current;
        }

        public void Dispose()
        {
            _space.Dispose();
        }

        public Task<ITransaction> BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public Task<T> Read(IQuery<T> query, CancellationToken token, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> TryRead(IQuery<T> query, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> Take(IQuery<T> query, CancellationToken token, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> TryTake(IQuery<T> query, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<T[]> Scan(IQuery<T> query, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> Count(IQuery<T> query, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task Add(T tuple, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task AddRange(IEnumerable<T> tuples, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }
    }
}