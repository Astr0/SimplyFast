using System;
using System.Collections.Generic;
using System.Diagnostics;
using SF.Data.Spaces.NotInUse;

namespace SF.Data.Spaces
{
    internal class LocalSpaceTable<T> : ILocalSpaceTable, ISyncSpaceTable<T> where T : class
    {
        public readonly ushort Id;
        private readonly LocalSpace _space;

        public LocalSpaceTable(LocalSpace space, ushort id, int transactionsCapacity)
        {
            _space = space;
            Id = id;
            _transactions = new LocalSpaceTableImpl<T>[transactionsCapacity];
            for (var i = 0; i < _transactions.Length; i++)
            {
                _transactions[i] = new LocalSpaceTableImpl<T>(new ArrayTupleStorage<T>(LocalSpaceConsts.TransactionWriteCapacity));
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

        private readonly LocalSpaceTableImpl<T> _root = new LocalSpaceTableImpl<T>(new ArrayTupleStorage<T>(LocalSpaceConsts.TableCapacity));
        private LocalSpaceTableImpl<T>[] _transactions;

        private LocalSpaceTableImpl<T> GetExistingParentImpl(ISyncTransaction transaction)
        {
            if (transaction == null)
                return _root;

            var localTransaction = (LocalTransaction) transaction;
            do
            {
                var impl = _transactions[localTransaction.Id];
                if (impl.Active)
                    return impl;
                localTransaction = localTransaction.Parent;
            } while (localTransaction != null);
            return _root;
        }

        private LocalSpaceTableImpl<T> GetImpl(ISyncTransaction transaction)
        {
            if (transaction == null)
                return _root;

            var localTransaction = (LocalTransaction)transaction;
            Debug.Assert(localTransaction == null || localTransaction.Space == _space, "Invalid transaction");
            Debug.Assert(localTransaction == null || localTransaction.Alive, "Dead transaction");

            var impl = _transactions[localTransaction.Id];
            if (impl.Active)
                return impl;
            // ok, this impl is dead, we need to init it
            if (localTransaction.Parent == null)
            {
                // this is just perf optimization for first level transactions
                // transaction don't know about changes in this table
                localTransaction.AddTable(this);
                impl.Parent = _root;
            }
            else
            {
                // transaction don't know about changes in this table
                var parentImpl = GetExistingParentImpl(localTransaction.Parent);
                if (parentImpl == _root)
                    localTransaction.AddTable(this);
                impl.Parent = parentImpl;
            }
            if (localTransaction.Children != null)
                SetNewParentForChildren(localTransaction.Children, impl);

            return impl;
        }

        private void SetNewParentForChildren(List<LocalTransaction> children, LocalSpaceTableImpl<T> impl)
        {
            foreach (var child in children)
            {
                var childImpl = _transactions[child.Id];
                if (childImpl.Active)
                {
                    // ok, we got alive child
                    // set it's parent
                    childImpl.Parent = impl;
                    continue;
                }
                // If child transaction is dead, go recursively
                // maybe it's child is alive
                if (child.Children != null)
                    SetNewParentForChildren(child.Children, impl);
            }
        }


        public void EnsureTransactionsCapacity(int count)
        {
            if (count < _transactions.Length)
                return;
            var oldSize = _transactions.Length;
            Array.Resize(ref _transactions, count);
            for (var i = oldSize; i < _transactions.Length; i++)
            {
                _transactions[i] = new LocalSpaceTableImpl<T>(new ArrayTupleStorage<T>(LocalSpaceConsts.TransactionWriteCapacity));
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
            var impl = _transactions[transaction.Id];
            if (impl.Active)
            {
                impl.Commit(target);
                return;
            }
            // ok, this transaction not found, what about it's children?
            if (transaction.Children != null)
            {
                foreach (var child in transaction.Children)
                {
                    CommitTransaction(child, target);
                }
            }
        }

        public void AbortTransaction(LocalTransaction transaction)
        {
            // find parts of this transaction or it's children
            var impl = _transactions[transaction.Id];
            if (impl.Active)
            {
                impl.Abort();
                return;
            }
            // ok, this transaction not found, what about it's children?
            if (transaction.Children != null)
            {
                foreach (var child in transaction.Children)
                {
                    AbortTransaction(child);
                }
            }
        }

        #endregion
    }
}