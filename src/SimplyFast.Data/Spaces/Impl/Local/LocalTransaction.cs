using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SF.Data.Spaces
{
    internal class LocalTransaction : ISyncTransaction
    {
        public readonly int Id;
        public readonly LocalTransaction Parent;
        public readonly LocalSpace Space;
        protected RootLocalTransaction Root;
        private List<LocalTransaction> _children;
        
        protected LocalTransaction(LocalSpace space, RootLocalTransaction root, LocalTransaction parent)
        {
            Space = space;
            Id = space.GetNextTransactionId();
            Root = root;
            Parent = parent;
            State = TransactionState.Running;
        }

        internal bool Alive => State == TransactionState.Running;
        public IEnumerable<LocalTransaction> Children => _children ?? Enumerable.Empty<LocalTransaction>();
        public TransactionState State { get; internal set; }
        
        public ISyncTransaction BeginTransaction()
        {
            var trans = new LocalTransaction(Space, Root, this);
            if (_children == null)
                _children = new List<LocalTransaction>(1);
            _children.Add(trans);
            return trans;
        }

        internal void AddTable(ILocalSpaceTable table)
        {
            Root.AddDependentTable(table);
        }

        public void Commit()
        {
            if (!Alive)
                throw new InvalidOperationException("Transaction already " + State);

            // commit trans
            Root.CommitTransaction(this);
            
            Cleanup(TransactionState.Commited);

            // remove this trans from parent
            Parent?._children.Remove(this);
        }


        public void Abort()
        {
            if (!Alive)
                throw new InvalidOperationException("Transaction already " + State);

            Root.AbortTransaction(this);

            Cleanup(TransactionState.Aborted);
            
            // remove trans from parent if any
            Parent?._children.Remove(this);
        }

        
        private void Cleanup(TransactionState state)
        {
            Debug.Assert(Alive, "Transaction not running");

            State = state;
            Space.Cleanup(Id);

            if (_children != null)
            {
                foreach (var child in Children)
                {
                    child.Cleanup(state);
                }
            }
        }

        void IDisposable.Dispose()
        {
            if (!Alive)
                return;
            Abort();
        }
    }

    internal class RootLocalTransaction : LocalTransaction
    {
        internal RootLocalTransaction(LocalSpace space) : base(space, null, null)
        {
            Root = this;
        }

        private readonly HashSet<ILocalSpaceTable> _tables = new HashSet<ILocalSpaceTable>();

        public void AddDependentTable(ILocalSpaceTable table)
        {
            _tables.Add(table);
        }

        public void CommitTransaction(LocalTransaction transaction)
        {
            foreach (var table in _tables)
            {
                table.CommitTransaction(transaction);
            }
        }

        public void AbortTransaction(LocalTransaction transaction)
        {
            foreach (var table in _tables)
            {
                table.AbortTransaction(transaction);
            }
        }
    }
}