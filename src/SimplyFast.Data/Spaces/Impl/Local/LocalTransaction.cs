using System;
using System.Collections.Generic;

namespace SF.Data.Spaces
{
    /// <summary>
    /// very important no to hold references on this stuff anywhere in LocalStorage
    /// since then finalizer won't fire and it'll be impossible to remove pending actions
    /// </summary>
    internal class LocalTransaction<T>: ILocalTransaction
        where T: class
    {
        public bool Alive;
        public readonly ICollection<TakenTuple<T>> Taken = new List<TakenTuple<T>>();
        public readonly TupleStorage<T> Written = new TupleStorage<T>();
        public readonly ICollection<LocalTransaction<T>> Children = new List<LocalTransaction<T>>(); 
        public readonly int Id;
        public readonly LocalSpace<T> Space;
        public readonly LocalTransaction<T> Parent;

        internal LocalTransaction(LocalSpace<T> space, int id, LocalTransaction<T> parent)
        {
            Space = space;
            Id = id;
            Parent = parent;
            Alive = true;
        }

        public void Commit()
        {
            // discard taken since they've been taken bugaga
            Space.CommitTransaction(this);
        }

        public ILocalTransaction BeginTransaction()
        {
            return Space.BeginTransaction(this);
        }

        public void Abort()
        {
            // Written stuff just get's discarded
            Space.AbortTransaction(this);
        }

        void IDisposable.Dispose()
        {
            if (!Alive)
                return;
            Abort();
        }
    }
}