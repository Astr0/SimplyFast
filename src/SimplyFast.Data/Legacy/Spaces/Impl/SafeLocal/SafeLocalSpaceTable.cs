using System;
using System.Threading;
using System.Threading.Tasks;
using SF.Threading;

namespace SF.Data.Legacy.Spaces
{
    internal class SafeLocalSpaceTable<T> : ISpaceTable<T> where T : class
    {
        private readonly Converter<T, T> _clone;
        private readonly SynchronizationContext _context;
        private readonly ISyncSpaceTable<T> _spaceTable;

        public SafeLocalSpaceTable(ISyncSpaceTable<T> spaceTable, Converter<T, T> clone,
            SynchronizationContext context = null)
        {
            _spaceTable = spaceTable;
            _clone = clone;
            _context = context ?? SynchronizationContext.Current;
        }

        public void Dispose()
        {
            _spaceTable.Dispose();
        }

        public T TryRead(IQuery<T> query, ISyncTransaction transaction = null)
        {
            return _context.Send(() => Clone(_spaceTable.TryRead(query, Unwrap(transaction))));
        }

        public T TryTake(IQuery<T> query, ISyncTransaction transaction = null)
        {
            return _context.Send(() => Clone(_spaceTable.TryTake(query, Unwrap(transaction))));
        }

        public void Read(IQuery<T> query, Action<T> callback, TimeSpan timeout, ISyncTransaction transaction = null)
        {
            _context.Send(x => _spaceTable.Read(query, t => callback(Clone(t)), timeout, Unwrap(transaction)), null);
        }

        public void Take(IQuery<T> query, Action<T> callback, TimeSpan timeout, ISyncTransaction transaction = null)
        {
            _context.Send(x => _spaceTable.Take(query, t => callback(Clone(t)), timeout, Unwrap(transaction)), null);
        }

        public T[] Scan(IQuery<T> query, ISyncTransaction transaction = null)
        {
            return _context.Send(() => Clone(_spaceTable.Scan(query, Unwrap(transaction))));
        }

        public int Count(IQuery<T> query, ISyncTransaction transaction = null)
        {
            return _context.Send(() => Count(query, transaction));
        }

        public void Add(T tuple, ISyncTransaction transaction = null)
        {
            _context.Send(x => _spaceTable.Add(Clone(tuple), transaction), null);
        }

        public void AddRange(T[] tuples, ISyncTransaction transaction = null)
        {
            _context.Send(x => _spaceTable.AddRange(Clone(tuples), transaction), null);
        }

        public Task<T> Read(IQuery<T> query, TimeSpan timeout, ITransaction transaction = null)
        {
            var tcs = new TaskCompletionSource<T>();
            _context.Post(x =>
            {
                try
                {
                    _spaceTable.Read(query, t =>
                    {
                        if (t != null)
                            tcs.TrySetResult(Clone(t));
                        else
                            tcs.TrySetCanceled();
                    }, timeout, Unwrap(transaction));
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, null);
            return tcs.Task;
        }

        public Task<T> TryRead(IQuery<T> query, ITransaction transaction = null)
        {
            return _context.PostTask(() => Clone(_spaceTable.TryRead(query, Unwrap(transaction))));
        }

        public Task<T> Take(IQuery<T> query, TimeSpan timeout, ITransaction transaction = null)
        {
            var tcs = new TaskCompletionSource<T>();
            _context.Post(x =>
            {
                try
                {
                    _spaceTable.Take(query, t =>
                    {
                        if (t != null)
                            tcs.TrySetResult(Clone(t));
                        else
                            tcs.TrySetCanceled();
                    }, timeout, Unwrap(transaction));
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, null);
            return tcs.Task;
        }

        public Task<T> TryTake(IQuery<T> query, ITransaction transaction = null)
        {
            return _context.PostTask(() => Clone(_spaceTable.TryTake(query, Unwrap(transaction))));
        }

        public Task<T[]> Scan(IQuery<T> query, ITransaction transaction = null)
        {
            return _context.PostTask(() => Clone(_spaceTable.Scan(query, Unwrap(transaction))));
        }

        public Task<int> Count(IQuery<T> query, ITransaction transaction = null)
        {
            return _context.PostTask(() => _spaceTable.Count(query, Unwrap(transaction)));
        }

        public Task Add(T tuple, ITransaction transaction = null)
        {
            return _context.PostTask(() => _spaceTable.Add(Clone(tuple), Unwrap(transaction)));
        }

        public Task AddRange(T[] tuples, ITransaction transaction = null)
        {
            return _context.PostTask(() => _spaceTable.AddRange(Clone(tuples), Unwrap(transaction)));
        }

        private static ISyncTransaction Unwrap(ITransaction transaction)
        {
            var trans = (SafeLocalTransaction) transaction;
            return trans?.SyncTransaction;
        }

        private static ISyncTransaction Unwrap(ISyncTransaction transaction)
        {
            var trans = (SafeLocalTransaction) transaction;
            return trans?.SyncTransaction;
        }

        #region clone stuff

        private T Clone(T obj)
        {
            return obj != null ? _clone(obj) : null;
        }

        private T[] Clone(T[] scan)
        {
            return Array.ConvertAll(scan, _clone);
        }

        #endregion
    }
}