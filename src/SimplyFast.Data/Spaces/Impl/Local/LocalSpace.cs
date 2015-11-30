using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SF.Data.Spaces
{
    internal class LocalSpace<T>: ILocalSpace<T> where T : class
    {
        private readonly TupleStorage<T> _storage = new TupleStorage<T>();
        private readonly WaitingActions<T> _waitingActions = new WaitingActions<T>();
        private int _nextTransactionId;
        
        public void Dispose()
        {
            // we're local, do nothing here
        }

        public ILocalTransaction BeginTransaction()
        {
            return new LocalTransaction<T>(this, _nextTransactionId++, null);
        }

        public T TryRead(IQuery<T> query, ILocalTransaction transaction = null)
        {
            var trans = GetTransaction(transaction);
            // try to read from transaction, including it parents
            while (trans != null)
            {
                var result = trans.Written.Read(query);
                if (result != null)
                    return result;
                trans = trans.Parent;
            }
            // no transaction or read failed, read from storage
            return _storage.Read(query);
        }

        public T TryTake(IQuery<T> query, ILocalTransaction transaction = null)
        {
            var rootTrans = GetTransaction(transaction);
            
            // no transactions, it's as simple as that
            if (rootTrans == null)
                return _storage.Take(query);

            // take what this transaction has written, no rollback stuff is required
            var taken = rootTrans.Written.Take(query);
            if (taken != null)
                return taken;

            // things get messy here since we should remember in what parent or storage we borrowed taken item =\
            var trans = rootTrans.Parent;
            // try to take from transaction including it's parents
            while (trans != null)
            {
                taken = trans.Written.Take(query);
                if (taken != null)
                {
                    rootTrans.Taken.Add(new TakenTuple<T>(trans, taken));
                    return taken;
                }
                trans = trans.Parent;
            }

            taken = _storage.Take(query);
            if (taken != null)
                rootTrans.Taken.Add(new TakenTuple<T>(null, taken));
            return taken;
        }

        public IDisposable Read(IQuery<T> query, Action<T> callback, ILocalTransaction transaction = null)
        {
            var readItem = TryRead(query, transaction);
            if (readItem != null)
            {
                callback(readItem);
                return DisposableEx.Null();
            }

            // add waiting action
            return AddWaitingAction(query, callback, transaction, false);
        }

        private IDisposable AddWaitingAction(IQuery<T> query, Action<T> callback, ILocalTransaction transaction, bool take)
        {
            var trans = GetTransaction(transaction);
            return trans != null 
                ? _waitingActions.Add(trans.Id, query, callback, take) 
                : _waitingActions.AddGlobal(query, callback, take);
        }

        public IDisposable Take(IQuery<T> query, Action<T> callback, ILocalTransaction transaction = null)
        {
            var takenItem = TryTake(query, transaction);
            if (takenItem != null)
            {
                callback(takenItem);
                return DisposableEx.Null();
            }

            // add waiting action
            return AddWaitingAction(query, callback, transaction, true);

        }

        public T[] Scan(IQuery<T> query, ILocalTransaction transaction = null)
        {
            var tuples = _storage.Scan(query);
            var trans = GetTransaction(transaction);
            // just iterate over tuples written by parent transactions
            while (trans != null)
            {
                tuples = tuples.Concat(trans.Written.Scan(query));
                trans = trans.Parent;
            }
            return tuples.ToArray();
        }

        public int Count(IQuery<T> query, ILocalTransaction transaction = null)
        {
            var tuples = _storage.Count(query);
            var trans = GetTransaction(transaction);
            while (trans != null)
            {
                tuples += trans.Written.Count(query);
                trans = trans.Parent;
            }
            return tuples;
        }

        public void Add(T tuple, ILocalTransaction transaction = null)
        {
            var rootTrans = GetTransaction(transaction);
            if (rootTrans != null)
            {
                // in transaction
                // try to take item by this transaction or it's parents
                var trans = rootTrans;
                while (trans != null)
                {
                    // if item was taken, nothing to add, just return
                    if (_waitingActions.Taken(tuple, trans.Id))
                        return;
                    trans = trans.Parent;
                }
                // item was not taken, add it to writes of this transaction
                rootTrans.Written.Add(tuple);
            }
            else
            {
                // out of transaction
                // try to take item by all transactions and non-transactions
                if (_waitingActions.Taken(tuple))
                    return;
                // item was not taken, add it to storage
                _storage.Add(tuple);
            }
        }

        public void AddRange(IEnumerable<T> tuples, ILocalTransaction transaction = null)
        {
            foreach (var tuple in tuples)
            {
                Add(tuple, transaction);
            }
        }

        #region Internal

        private LocalTransaction<T> GetTransaction(ILocalTransaction transaction)
        {
            var result = (LocalTransaction<T>)transaction;
            Debug.Assert(result == null || result.Space == this, "Invalid transaction");
            Debug.Assert(result == null || result.Alive, "Dead transaction");
            return result;
        }

        /// <summary>
        /// Called by transaction when aborted
        /// </summary>
        internal void AbortTransaction(LocalTransaction<T> trans)
        {
            // do abort stuff
            DoAbortTransaction(trans);

            // remove trans from parent if any
            var parent = trans.Parent;
            if (parent != null)
                parent.Children.Remove(trans);

        }

        private void DoAbortTransaction(LocalTransaction<T> trans)
        {
            Debug.Assert(trans.Alive, "trans.Alive");
            
            // cleanup trans so childs won't spam taken items
            Cleanup(trans);

            // abort child transactions
            foreach (var child in trans.Children)
            {
                DoAbortTransaction(child);
            }

            // return taken items to parent trans or storage
            foreach (var pair in trans.Taken)
            {
                var target = pair.Transaction;
                if (target == null || target.Alive)
                    Add(pair.Tuple, target);
            }
        }

        /// <summary>
        /// Called by transaction when commited
        /// </summary>
        internal void CommitTransaction(LocalTransaction<T> trans)
        {
            var parent = trans.Parent;

            // commit trans to parent trans
            CommitTransaction(trans, parent);

            // remove this trans from parent
            if (parent != null)
                parent.Children.Remove(trans);
        }

        private void CommitTransaction(LocalTransaction<T> trans, LocalTransaction<T> target)
        {
            Debug.Assert(trans.Alive, "trans.Alive");
            
            Cleanup(trans);
            
            // commit child transactions?
            foreach (var child in trans.Children)
            {
                CommitTransaction(child, target);
            }

            // write items to target transaction
            AddRange(trans.Written, target);
        }

        internal LocalTransaction<T> BeginTransaction(LocalTransaction<T> trans)
        {
            Debug.Assert(trans.Alive, "trans.Alive");
            var newTrans = new LocalTransaction<T>(this, _nextTransactionId++, trans);
            trans.Children.Add(newTrans);
            return newTrans;
        }

        private void Cleanup(LocalTransaction<T> trans)
        {
            trans.Alive = false;
            _waitingActions.Cleanup(trans.Id);
            // todo: cleanup waiting list
        }

        #endregion
    }
}