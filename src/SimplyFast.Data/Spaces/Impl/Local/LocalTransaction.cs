using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SF.Data.Spaces
{
    internal class LocalTransaction : ISyncTransaction
    {
        public readonly int Id;
        public readonly LocalTransaction Parent;
        public readonly LocalSpace Space;
        protected RootLocalTransaction Root;

        public List<LocalTransaction> Children;
        
        protected LocalTransaction(LocalSpace space, RootLocalTransaction root, LocalTransaction parent)
        {
            Space = space;
            Id = space.GetNextTransactionId();
            Root = root;
            Parent = parent;
            State = TransactionState.Running;
        }

        public TransactionState State { get; internal set; }
        
        public ISyncTransaction BeginTransaction()
        {
            var trans = new LocalTransaction(Space, Root, this);
            if (Children == null)
                Children = new List<LocalTransaction>(1);
            Children.Add(trans);
            return trans;
        }

        internal void AddTable(ILocalSpaceTable table)
        {
            Root.AddDependentTable(table);
        }

        public void Commit()
        {
            if (State != TransactionState.Running)
                throw new InvalidOperationException("Transaction already " + State);

            // commit trans
            Root.CommitTransaction(this);
            
            Cleanup(TransactionState.Commited);
            
            // remove trans from parent if any
            Parent?.Children.Remove(this);
        }


        public void Abort()
        {
            if (State != TransactionState.Running)
                throw new InvalidOperationException("Transaction already " + State);

            Root.AbortTransaction(this);

            Cleanup(TransactionState.Aborted);
            
            // remove trans from parent if any
            Parent?.Children.Remove(this);
        }

        
        private void Cleanup(TransactionState state)
        {
            State = state;
            Space.Cleanup(Id);

            if (Children != null)
            {
                foreach (var child in Children)
                {
                    child.Cleanup(state);
                }
            }

        }

        void IDisposable.Dispose()
        {
            if (State != TransactionState.Running)
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
            Debug.Assert(!_tables.Contains(table), "Table added more than once!");
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