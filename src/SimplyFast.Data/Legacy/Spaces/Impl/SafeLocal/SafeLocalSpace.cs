using System;
using System.Threading;
using System.Threading.Tasks;
using SF.Threading;

namespace SF.Data.Legacy.Spaces
{
    internal class SafeLocalSpace : ISpace
    {
        private readonly Converter<object, object> _clone;
        private readonly SynchronizationContext _context;
        private readonly ISyncSpace _space;

        public SafeLocalSpace(ISyncSpace space, Converter<object, object> clone, SynchronizationContext context = null)
        {
            _space = space;
            _clone = clone;
            _context = context ?? SynchronizationContext.Current;
        }

        ISyncSpaceTable<T> ISyncSpace.GetTable<T>(ushort id)
        {
            return _context.Send(() => Wrap(_space.GetTable<T>(id)));
        }

        public Task<ISpaceTable<T>> GetTable<T>(ushort id) where T : class
        {
            return _context.PostTask(() => Wrap(_space.GetTable<T>(id)));
        }

        ISyncTransaction ISyncSpace.BeginTransaction()
        {
            return _context.Send(() => Wrap(_space.BeginTransaction()));
        }

        public Task<ITransaction> BeginTransaction()
        {
            return _context.PostTask(() => Wrap(_space.BeginTransaction()));
        }

        private ISpaceTable<T> Wrap<T>(ISyncSpaceTable<T> table)
            where T : class
        {
            return new SafeLocalSpaceTable<T>(table, x => (T) _clone(x), _context);
        }

        #region transaction stuff

        private ITransaction Wrap(ISyncTransaction transaction)
        {
            return new SafeLocalTransaction(this, transaction);
        }

        internal Task<ITransaction> BeginTransaction(ISyncTransaction syncTransaction)
        {
            return _context.PostTask(syncTransaction, x => Wrap(x.BeginTransaction()));
        }

        internal void DisposeTransaction(ISyncTransaction syncTransaction)
        {
            _context.Send(syncTransaction, x => x.Dispose());
        }

        internal Task Abort(ISyncTransaction syncTransaction)
        {
            return _context.PostTask(syncTransaction, x => x.Abort());
        }

        internal Task Commit(ISyncTransaction syncTransaction)
        {
            return _context.PostTask(syncTransaction, x => x.Commit());
        }

        internal ISyncTransaction BeginTransactionSync(ISyncTransaction syncTransaction)
        {
            return _context.Send(syncTransaction, x => Wrap(x.BeginTransaction()));
        }

        internal void CommitSync(ISyncTransaction syncTransaction)
        {
            _context.Send(syncTransaction, x => x.Commit());
        }

        internal void AbortSync(ISyncTransaction syncTransaction)
        {
            _context.Send(syncTransaction, x => x.Abort());
        }

        #endregion
    }
}