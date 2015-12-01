using System;
using System.Diagnostics;

namespace SF.Data.Spaces
{
    internal class LocalSpaceTable<T> : ILocalSpaceTable, ISyncSpaceTable<T> where T : class
    {
        public readonly ushort Id;
        private readonly LocalSpace _space;

        public LocalSpaceTable(LocalSpace space, ushort id)
        {
            _space = space;
            Id = id;
            _transactions = new LocalSpaceTableImpl<T>[LocalSpaceConsts.TransactionsCapacity];
            for (var i = 0; i < _transactions.Length; i++)
            {
                _transactions[i] = new LocalSpaceTableImpl<T>();
            }
        }

        public void Dispose()
        {
            // we're local, do nothing here
        }

        public T TryRead(IQuery<T> query, ISyncTransaction transaction = null)
        {
            var impl = GetExistingParentImpl(transaction);
            return impl.TryRead(query);
        }

        public T TryTake(IQuery<T> query, ISyncTransaction transaction = null)
        {
            var impl = GetImpl(transaction);
            return impl.TryTake(query);
        }

        public void Read(IQuery<T> query, Action<T> callback, TimeSpan timeout, ISyncTransaction transaction = null)
        {
            var impl = GetImpl(transaction);
            impl.Read(query, callback, timeout);
        }

        public void Take(IQuery<T> query, Action<T> callback, TimeSpan timeout, ISyncTransaction transaction = null)
        {
            var impl = GetImpl(transaction);
            impl.Take(query, callback, timeout);
        }

        public T[] Scan(IQuery<T> query, ISyncTransaction transaction = null)
        {
            var impl = GetExistingParentImpl(transaction);
            return impl.Scan(query);
        }

        public int Count(IQuery<T> query, ISyncTransaction transaction = null)
        {
            var impl = GetExistingParentImpl(transaction);
            return impl.Count(query);
        }

        public void Add(T tuple, ISyncTransaction transaction = null)
        {
            var impl = GetImpl(transaction);
            impl.Add(tuple);
        }

        public void AddRange(T[] tuples, ISyncTransaction transaction = null)
        {
            var impl = GetImpl(transaction);
            impl.AddRange(tuples);
        }

        #region Internal

        private readonly LocalSpaceTableImpl<T> _root = new LocalSpaceTableImpl<T>();
        private LocalSpaceTableImpl<T>[] _transactions;

        private LocalSpaceTableImpl<T> GetExistingParentImpl(ISyncTransaction transaction)
        {
            var localTransaction = (LocalTransaction)transaction;
            Debug.Assert(localTransaction == null || localTransaction.Space == _space, "Invalid transaction");
            Debug.Assert(localTransaction == null || localTransaction.Alive, "Dead transaction");
            while (localTransaction != null)
            {
                if (localTransaction.Id < _transactions.Length)
                {
                    var impl = _transactions[localTransaction.Id];
                    if (impl.Active)
                        return impl;
                }
                localTransaction = localTransaction.Parent;
            }
            return _root;
        }

        private LocalSpaceTableImpl<T> GetImpl(ISyncTransaction transaction)
        {
            if (transaction == null)
                return _root;

            var localTransaction = (LocalTransaction)transaction;
            Debug.Assert(localTransaction == null || localTransaction.Space == _space, "Invalid transaction");
            Debug.Assert(localTransaction == null || localTransaction.Alive, "Dead transaction");
            if (localTransaction.Id >= _transactions.Length)
            {
                var oldSize = _transactions.Length;
                var newSize = Math.Max(oldSize*2, localTransaction.Id + 1);
                Array.Resize(ref _transactions, newSize);
                for (var i = 0; i < _transactions.Length; i++)
                {
                    _transactions[i] = new LocalSpaceTableImpl<T>();
                }
            }

            var impl = _transactions[localTransaction.Id];
            if (impl.Active)
                return impl;
            // ok, this impl is dead, we need to init it
            var parent = GetExistingParentImpl(localTransaction.Parent);
            if (parent == _root)
            {
                // transaction don't know about changes in this table
                localTransaction.AddTable(this);
            }
            impl.Parent = parent;
            SetNewParentForChildren(localTransaction, impl);
            
            return impl;
        }

        private void SetNewParentForChildren(LocalTransaction trans, LocalSpaceTableImpl<T> impl)
        {
            foreach (var child in trans.Children)
            {
                if (child.Id < _transactions.Length)
                {
                    var childImpl = _transactions[child.Id];
                    if (childImpl.Active)
                    {
                        // ok, we got alive child
                        // set it's parent
                        childImpl.Parent = impl;
                        continue;
                    }
                }
                // If child transaction is dead, go recursively
                // maybe it's child is alive
                SetNewParentForChildren(child, impl);
            }
        }


        public void CommitTransaction(LocalTransaction transaction)
        {
            var target = GetImpl(transaction.Parent);
            CommitTransaction(transaction, target);
        }

        private void CommitTransaction(LocalTransaction transaction, LocalSpaceTableImpl<T> target)
        {
            // find parts of this transaction or it's children
            if (transaction.Id < _transactions.Length)
            {
                var impl = _transactions[transaction.Id];
                if (impl.Active)
                {
                    impl.Commit(target);
                    return;
                }
            }
            // ok, this transaction not found, what about it's children?
            foreach (var child in transaction.Children)
            {
                CommitTransaction(child, target);
            }
        }

        public void AbortTransaction(LocalTransaction transaction)
        {
            // find parts of this transaction or it's children
            if (transaction.Id < _transactions.Length)
            {
                var impl = _transactions[transaction.Id];
                if (impl.Active)
                {
                    impl.Abort();
                    return;
                }
            }
            // ok, this transaction not found, what about it's children?
            foreach (var child in transaction.Children)
            {
                AbortTransaction(child);
            }
        }

        #endregion
    }
}