using System.Threading.Tasks;

namespace SF.Data.Legacy.Spaces
{
    internal class SafeLocalTransaction : ITransaction
    {
        private readonly SafeLocalSpace _space;
        public readonly ISyncTransaction SyncTransaction;

        public SafeLocalTransaction(SafeLocalSpace space, ISyncTransaction syncTransaction)
        {
            _space = space;
            SyncTransaction = syncTransaction;
        }

        public void Dispose()
        {
            if (State != TransactionState.Running)
                return;
            _space.DisposeTransaction(SyncTransaction);
        }

        ISyncTransaction ISyncTransaction.BeginTransaction()
        {
            return _space.BeginTransactionSync(SyncTransaction);
        }

        public TransactionState State => SyncTransaction.State;

        public Task<ITransaction> BeginTransaction()
        {
            return _space.BeginTransaction(SyncTransaction);
        }

        public Task Abort()
        {
            return _space.Abort(SyncTransaction);
        }

        void ISyncTransaction.Commit()
        {
            _space.CommitSync(SyncTransaction);
        }

        void ISyncTransaction.Abort()
        {
            _space.AbortSync(SyncTransaction);
        }

        public Task Commit()
        {
            return _space.Commit(SyncTransaction);
        }
    }
}