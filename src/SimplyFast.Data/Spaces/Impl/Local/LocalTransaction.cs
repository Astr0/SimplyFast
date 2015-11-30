using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SF.Data.Spaces
{
    internal class LocalTransaction : ISyncTransaction
    {
        //public readonly ICollection<LocalTransaction> Children = new List<LocalTransaction>();
        private readonly List<ILocalTransactionData> _data = new List<ILocalTransactionData>(4);
        public readonly int Id;
        public readonly LocalTransaction Parent;
        public readonly LocalSpace Space;
        private ICollection<LocalTransaction> _children;

        internal LocalTransaction(LocalSpace space, LocalTransaction parent = null)
        {
            Space = space;
            Id = space.GetNextTransactionId();
            Parent = parent;
            State = TransactionState.Running;
        }

        internal bool Alive => State == TransactionState.Running;
        public IEnumerable<LocalTransaction> Children => _children ?? Enumerable.Empty<LocalTransaction>();
        public TransactionState State { get; internal set; }

        public void Commit()
        {
            var parent = Parent;

            Cleanup(TransactionState.Commited);

            // commit trans to parent trans
            DoCommit(parent);

            // remove this trans from parent
            parent?._children?.Remove(this);
        }

        public ISyncTransaction BeginTransaction()
        {
            var trans = new LocalTransaction(Space, this);
            if (_children == null)
                _children = new List<LocalTransaction>(1);
            _children.Add(trans);
            return trans;
        }

        public void Abort()
        {
            Cleanup(TransactionState.Aborted);

            DoAbort();

            // remove trans from parent if any
            Parent?._children?.Remove(this);
        }

        void IDisposable.Dispose()
        {
            if (!Alive)
                return;
            Abort();
        }

        private void DoCommit(LocalTransaction target)
        {
            // commit child transactions?
            foreach (var child in Children)
            {
                child.DoCommit(target);
            }

            // commit data
            foreach (var data in _data)
            {
                data.Commit(target);
            }
        }

        private void Cleanup(TransactionState state)
        {
            Debug.Assert(Alive, "Transaction not running");

            State = state;
            foreach (var data in _data)
            {
                data.Cleanup();
            }

            foreach (var child in Children)
            {
                child.Cleanup(state);
            }
        }

        private void DoAbort()
        {
            // abort child transactions
            foreach (var child in Children)
            {
                child.DoAbort();
            }

            // abort tables
            foreach (var data in _data)
            {
                data.Abort();
            }
        }

        public LocalTransactionData<T> GetData<T>(ushort id) where T : class
        {
            return (LocalTransactionData<T>) _data.Find(x => x.TableId == id);
        }

        public LocalTransactionData<T> GetOrAddData<T>(LocalSpaceTable<T> table) where T : class
        {
            var data = GetData<T>(table.Id);
            if (data != null)
                return data;
            data = new LocalTransactionData<T>(this, table);
            _data.Add(data);
            return data;
        }

        public T ReadToParent<T>(ushort id, IQuery<T> query) where T : class
        {
            var result = GetData<T>(id)?.Written.Read(query);
            return result ?? Parent?.ReadToParent(id, query);
        }

        public T Take<T>(ushort id, IQuery<T> query) where T : class
        {
            return GetData<T>(id)?.Written.Take(query);
        }

        public void AddTaken<T>(LocalSpaceTable<T> table, TakenTuple<T> taken) where T : class
        {
            GetOrAddData(table).Taken.Add(taken);
        }

        public void AddWritten<T>(LocalSpaceTable<T> table, T tuple) where T : class
        {
            GetOrAddData(table).Written.Add(tuple);
        }

        public IEnumerable<T> Scan<T>(ushort id, IQuery<T> query) where T : class
        {
            return GetData<T>(id)?.Written.Scan(query);
        }

        public int Count<T>(ushort id, IQuery<T> query) where T : class
        {
            return GetData<T>(id)?.Written.Count(query) ?? 0;
        }
    }

    internal interface ILocalTransactionData
    {
        ushort TableId { get; }
        void Abort();
        void Commit(LocalTransaction target);
        void Cleanup();
    }

    /// <summary>
    ///     very important no to hold references on this stuff anywhere in LocalStorage
    ///     since then finalizer won't fire and it'll be impossible to remove pending actions
    /// </summary>
    internal class LocalTransactionData<T> : ILocalTransactionData
        where T : class
    {
        private readonly LocalSpaceTable<T> _table;
        private readonly LocalTransaction _transaction;
        public readonly ICollection<TakenTuple<T>> Taken = new List<TakenTuple<T>>();
        public readonly TupleStorage<T> Written = new TupleStorage<T>();

        public LocalTransactionData(LocalTransaction transaction, LocalSpaceTable<T> table)
        {
            _transaction = transaction;
            _table = table;
        }

        public int Id => _transaction.Id;
        public ushort TableId => _table.Id;

        public void Abort()
        {
            _table.AbortTransaction(this);
        }

        public void Commit(LocalTransaction target)
        {
            _table.CommitTransaction(this, target);
        }

        public void Cleanup()
        {
            _table.CleanupTransaction(this);
        }
    }
}