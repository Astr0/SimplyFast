using System;
using System.Collections.Generic;

namespace SF.Data.Spaces
{
    internal class LocalSpaceTableImpl<T> where T : class
    {
        private readonly TupleStorage<T> _written = new TupleStorage<T>(); 
        private readonly ICollection<TakenTuple<T>> _taken = new List<TakenTuple<T>>();
        private const int WaitingListCapacity = 1;
        private readonly List<WaitingAction> _waitingActions = new List<WaitingAction>(WaitingListCapacity);
        
        private readonly List<LocalSpaceTableImpl<T>> _children = new List<LocalSpaceTableImpl<T>>();
        private LocalSpaceTableImpl<T> _parent;

        public bool Active => _parent != null;

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

        //public IEnumerable<LocalSpaceTableImpl<T>> Children => _children;

        
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
                return;
            }

            // add waiting action
            _waitingActions.Add(new WaitingAction(query, callback, DateTime.UtcNow.Add(timeout), false));
        }

        public void Take(IQuery<T> query, Action<T> callback, TimeSpan timeout)
        {
            var takenItem = TryTake(query);
            if (takenItem != null)
            {
                callback(takenItem);
                return;
            }

            // add waiting action
            _waitingActions.Add(new WaitingAction(query, callback, DateTime.UtcNow.Add(timeout), true));
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
            //if (_children.Count == 0 && _waitingActions.Count == 0)
            //{
            //    // no waiting actions 
            //    _written.Add(tuple);
            //    return;
            //}
            // try to take item by this transaction or it's childs
            var source = this;
            PendingReadAndTake(tuple, ref source);
            if (source != null)
                _written.Add(tuple);
        }

        private void PendingReadAndTake(T tuple, ref LocalSpaceTableImpl<T> source)
        {
            for (var i = _waitingActions.Count - 1; i >= 0; i--)
            {
                var action = _waitingActions[i];
                // check expiration of all actions
                if (action.Expire < DateTime.UtcNow)
                {
                    action.Callback(null);
                    _waitingActions.RemoveAt(i);
                    continue;
                }
                // ignore take actions here
                if (action.Take && source == null)
                    continue;
                if (!action.Query.Match(tuple))
                    continue;
                action.Callback(tuple);
                _waitingActions.RemoveAt(i);
                if (!action.Take)
                    continue;
                if (this != source)
                    _taken.Add(new TakenTuple<T>(source, tuple));
                source = null;
            }
            foreach (var child in _children)
            {
                child.PendingReadAndTake(tuple, ref source);
            }
        }

        public void AddRange(T[] tuples)
        {
            foreach (var tuple in tuples)
            {
                Add(tuple);
            }
        }

        private void AddRange(IEnumerable<T> tuples)
        {
            foreach (var tuple in tuples)
            {
                Add(tuple);
            }
        }

        public void Abort()
        {
            // remove itself
            _parent?._children.Remove(this);
            DoAbort();
        }

        private void DoAbort()
        {
            StartCleanup();

            // abort children
            foreach (var child in _children)
            {
                child.DoAbort();
            }

            // abort this
            foreach (var takenTuple in _taken)
            {
                var target = takenTuple.TableImpl;
                if (target.Active)
                    target.Add(takenTuple.Tuple);
            }

            EndCleanup();
        }

        private void StartCleanup()
        {
            _waitingActions.Clear();
            _parent = null;
        }

        private void EndCleanup()
        {
            _taken.Clear();
            _written.Clear();
            _children.Clear();
        }

        public void Commit(LocalSpaceTableImpl<T> target)
        {
            // remove itself
            _parent?._children.Remove(this);
            DoCommit(target);
        }

        private void DoCommit(LocalSpaceTableImpl<T> target)
        {
            StartCleanup();

            // commit children
            foreach (var child in _children)
            {
                child.DoCommit(target);
            }

            target.AddRange(_written);

            EndCleanup();
        }

        private class WaitingAction
        {
            public readonly Action<T> Callback;
            public readonly DateTime Expire;
            public readonly IQuery<T> Query;
            public readonly bool Take;

            public WaitingAction(IQuery<T> query, Action<T> callback, DateTime expire, bool take)
            {
                Query = query;
                Callback = callback;
                Take = take;
                Expire = expire;
            }
        }
    }
}