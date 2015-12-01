using System;
using System.Collections.Generic;

namespace SF.Data.Spaces.New
{
    public class LocalSpaceTableImpl<T> where T : class
    {
        private readonly TupleStorage<T> _written = new TupleStorage<T>(); 
        private readonly ICollection<TakenTuple<T>> _taken = new List<TakenTuple<T>>();
        private readonly WaitingActions<T> _waitingActions = new WaitingActions<T>();

        private readonly List<LocalSpaceTableImpl<T>> _children = new List<LocalSpaceTableImpl<T>>();
        private LocalSpaceTableImpl<T> _parent;

        public LocalSpaceTableImpl<T> Parent
        {
            get { return _parent; }
            set
            {
                _parent?._children.Remove(this);
                _parent = value;
                _parent?._children.Add(this);
            }
        }  

        public IEnumerable<LocalSpaceTableImpl<T>> Children => _children;

        
        public T TryRead(IQuery<T> query)
        {
            var source = this;
            do
            {
                var result = source._written.Read(query);
                if (result != null)
                    return result;
                source = source._parent;
            } while (source != null);
            return null;
        }

        public T TryTake(IQuery<T> query)
        {
            var taken = _written.Take(query);
            if (taken != null)
                return taken;
            var borrower = _parent;
            while (borrower != null)
            {
                taken = borrower._written.Take(query);
                if (taken != null)
                {
                    _taken.Add(new TakenTuple<T>(borrower, taken));
                    return taken;
                }
                borrower = borrower._parent;
            }
            return null;
        }

        public void Read(IQuery<T> query, Action<T> callback, TimeSpan timeout)
        {
            var readItem = TryRead(query);
            if (readItem != null)
            {
                callback(readItem);
            }

            // add waiting action
            _waitingActions.Add(query, callback, DateTime.UtcNow.Add(timeout), false);
        }

        public void Take(IQuery<T> query, Action<T> callback, TimeSpan timeout)
        {
            var takenItem = TryTake(query);
            if (takenItem != null)
            {
                callback(takenItem);
            }

            // add waiting action
            _waitingActions.Add(query, callback, DateTime.UtcNow.Add(timeout), true);
        }

        public T[] Scan(IQuery<T> query)
        {
            var result = new List<T>();
            var impl = this;
            do
            {
                result.AddRange(impl._written.Scan(query));
                impl = impl._parent;
            } while (impl != null);
            return result.ToArray();
        }

        public int Count(IQuery<T> query)
        {
            var impl = this;
            var count = 0;
            do
            {
                count += impl._written.Count(query);
                impl = impl._parent;
            } while (impl != null);
            return count;
        }

        public void Add(T tuple)
        {
            // try to take item by this transaction or it's childs
            var now = DateTime.UtcNow;
            if (Taken(tuple, now, false))
                return;
            _written.Add(tuple);
        }

        private bool Taken(T tuple, DateTime now, bool taken)
        {
            taken = _waitingActions.Taken(tuple, now, taken);
            // ReSharper disable once LoopCanBeConvertedToQuery
            
            // item is available to child transactions
            // iterate all the transactions so everyone can read it
            foreach (var child in Children)
            {
                taken = child.Taken(tuple, now, taken);
            }
            return taken;
        }

        public void AddRange(T[] tuples)
        {
            foreach (var tuple in tuples)
            {
                Add(tuple);
            }
        }
    }
}