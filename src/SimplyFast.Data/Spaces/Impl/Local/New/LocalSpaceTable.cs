//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;

//namespace SF.Data.Spaces.New
//{
//    internal class LocalSpaceTable<T> : ISyncSpaceTable<T> where T : class
//    {
//        public readonly ushort Id;
//        private readonly LocalSpace _space;

//        public LocalSpaceTable(LocalSpace space, ushort id)
//        {
//            _space = space;
//            Id = id;
//        }

//        public void Dispose()
//        {
//            // we're local, do nothing here
//        }

//        public T TryRead(IQuery<T> query, ISyncTransaction transaction = null)
//        {
//            var trans = GetTransaction(transaction);
//            var result = trans?.ReadToParent(Id, query);
//            return result ?? _storage.Read(query);
//        }

//        public T TryTake(IQuery<T> query, ISyncTransaction transaction = null)
//        {
//            var rootTrans = GetTransaction(transaction);

//            // no transactions, it's as simple as that
//            if (rootTrans == null)
//                return _storage.Take(query);

//            // take what this transaction has written, no rollback stuff is required
//            var taken = rootTrans.Take(Id, query);
//            if (taken != null)
//                return taken;

//            // things get messy here since we should remember in what parent or storage we borrowed taken item =\
//            var trans = rootTrans.Parent;
//            // try to take from transaction including it's parents
//            while (trans != null)
//            {
//                taken = trans.Take(Id, query);
//                if (taken != null)
//                {
//                    rootTrans.AddTaken(this, new TakenTuple<T>(trans, taken));
//                    return taken;
//                }
//                trans = trans.Parent;
//            }

//            taken = _storage.Take(query);
//            if (taken != null)
//                rootTrans.AddTaken(this, new TakenTuple<T>(null, taken));
//            return taken;
//        }

//        public void Read(IQuery<T> query, Action<T> callback, TimeSpan timeout, ISyncTransaction transaction = null)
//        {
//            var readItem = TryRead(query, transaction);
//            if (readItem != null)
//            {
//                callback(readItem);
//            }

//            // add waiting action
//            AddWaitingAction(query, callback, timeout, transaction, false);
//        }

//        public void Take(IQuery<T> query, Action<T> callback, TimeSpan timeout, ISyncTransaction transaction = null)
//        {
//            var takenItem = TryTake(query, transaction);
//            if (takenItem != null)
//            {
//                callback(takenItem);
//            }

//            // add waiting action
//            AddWaitingAction(query, callback, timeout, transaction, true);
//        }

//        public T[] Scan(IQuery<T> query, ISyncTransaction transaction = null)
//        {
//            var tuples = _storage.Scan(query);
//            var trans = GetTransaction(transaction);
//            // just iterate over tuples written by parent transactions
//            while (trans != null)
//            {
//                var scan = trans.Scan(Id, query);
//                if (scan != null)
//                    tuples = tuples.Concat(trans.Scan(Id, query));
//                trans = trans.Parent;
//            }
//            return tuples.ToArray();
//        }

//        public int Count(IQuery<T> query, ISyncTransaction transaction = null)
//        {
//            var tuples = _storage.Count(query);
//            var trans = GetTransaction(transaction);
//            while (trans != null)
//            {
//                tuples += trans.Count(Id, query);
//                trans = trans.Parent;
//            }
//            return tuples;
//        }

//        public void Add(T tuple, ISyncTransaction transaction = null)
//        {
//            var trans = GetTransaction(transaction);
//            if (trans != null)
//            {
//                // in transaction
//                // try to take item by this transaction or it's childs
//                if (ChildTaken(trans, tuple, false))
//                    return;

//                // item was not taken, add it to writes of this transaction
//                trans.AddWritten(this, tuple);
//            }
//            else
//            {
//                // out of transaction
//                // try to take item by all transactions and non-transactions
//                if (_waitingActions.Taken(tuple))
//                    return;
//                // item was not taken, add it to storage
//                _storage.Add(tuple);
//            }
//        }

//        public void AddRange(T[] tuples, ISyncTransaction transaction = null)
//        {
//            AddRange((IEnumerable<T>)tuples, transaction);
//        }

//        private void AddWaitingAction(IQuery<T> query, Action<T> callback, TimeSpan timeout,
//            ISyncTransaction transaction, bool take)
//        {
//            var trans = GetTransaction(transaction);
//            if (trans != null)
//            {
//                trans.GetOrAddData(this);
//                _waitingActions.Add(trans.Id, query, callback, timeout, take);
//            }
//            else
//            {
//                _waitingActions.AddGlobal(query, callback, timeout, take);
//            }
//        }

//        private bool ChildTaken(LocalTransaction trans, T tuple, bool taken)
//        {
//            // item is available to child transactions
//            // iterate all the transactions so everyone can read it
//            taken = _waitingActions.Taken(tuple, trans.Id, taken);
//            // ReSharper disable once LoopCanBeConvertedToQuery
//            foreach (var transaction in trans.Children)
//            {
//                taken = ChildTaken(transaction, tuple, taken);
//            }
//            return taken;
//        }

//        public void AddRange(IEnumerable<T> tuples, ISyncTransaction transaction = null)
//        {
//            foreach (var tuple in tuples)
//            {
//                Add(tuple, transaction);
//            }
//        }

//        #region Internal

//        private LocalSpaceTableImpl<T> _root = new LocalSpaceTableImpl<T>();
        
//        private LocalSpaceTableImpl<T>[] _transactions = new LocalSpaceTableImpl<T>[0]; 

//        private LocalSpaceTableImpl<T> GetTransaction(ISyncTransaction transaction)
//        {
//            var result = (LocalTransaction)transaction;
//            Debug.Assert(result == null || result.Space == _space, "Invalid transaction");
//            Debug.Assert(result == null || result.Alive, "Dead transaction");
//            return result;
//        }

//        /// <summary>
//        ///     Called by transaction when aborted
//        /// </summary>
//        internal void AbortTransaction(LocalTransactionData<T> trans)
//        {
//            // return taken items to parent trans or storage
//            foreach (var pair in trans.Taken)
//            {
//                var target = pair.Transaction;
//                if (target == null || target.Alive)
//                    Add(pair.Tuple, target);
//            }
//        }

//        /// <summary>
//        ///     Called by transaction when commited
//        /// </summary>
//        internal void CommitTransaction(LocalTransactionData<T> trans, LocalTransaction target)
//        {
//            // write items to target transaction
//            AddRange(trans.Written, target);
//        }

//        public void CleanupTransaction(LocalTransactionData<T> trans)
//        {
//            _waitingActions.Cleanup(trans.Id);
//        }

//        #endregion
//    }
//}