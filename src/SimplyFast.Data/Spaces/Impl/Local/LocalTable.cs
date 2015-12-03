using System.Collections.Generic;
using System.Diagnostics;

namespace SF.Data.Spaces.Local
{
    internal class LocalTable<T>: ILocalTable
    {
        private readonly ITupleStorage<T> _written;
        private readonly List<TakenTuple<T>> _taken = new List<TakenTuple<T>>(LocalSpaceConsts.TransactionWrittenCapacity); 
        private LocalTable<T> _parent;


        #region Factory

        public static LocalTable<T> GetRoot()
        {
            return new LocalTable<T>(new ArrayTupleStorage<T>(LocalSpaceConsts.RootWrittenCapacity));
        }

        private static readonly Stack<LocalTable<T>> _cache = new Stack<LocalTable<T>>(LocalSpaceConsts.TransactionCacheCapacity);
        public static LocalTable<T> GetTransactional(int hierarchyLevel, LocalTable<T> parent)
        {
            var trans = _cache.Count != 0 ? _cache.Pop() : new LocalTable<T>(new ArrayTupleStorage<T>(LocalSpaceConsts.TransactionWrittenCapacity));
            trans.HierarchyLevel = hierarchyLevel;
            trans._parent = parent;
            return trans;
        }

        #endregion


        private LocalTable(ITupleStorage<T> written)
        {
            _written = written;
        }

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
            return default(T);
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
            return default(T);
        }

        public IReadOnlyList<T> Scan(IQuery<T> query)
        {
            var result = new List<T>();
            var impl = this;
            do
            {
                impl._written.Scan(query, result.Add);
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
            // TODO: Check pending actions
            _written.Add(tuple);
        }

        private void AddRange(T[] tuples, int count)
        {
            if (count == 0)
                return;
            // TODO: Check pending actions
            _written.AddRange(tuples, count);
        }

        public void AddRange(T[] tuples)
        {
            AddRange(tuples, tuples.Length);
        }

        public ILocalTable Parent => _parent;
        public int HierarchyLevel { get; private set; }

        public void Commit()
        {
            Debug.Assert(_parent != null);
            int count;
            var written = _written.GetArray(out count);
            _parent.AddRange(written, count);

            _parent = null;
            _written.Clear();
            _taken.Clear();
            _cache.Push(this);
        }

        public void Abort()
        {
            Debug.Assert(_parent != null);
            foreach (var takenTuple in _taken)
            {
                takenTuple.Abort();
            }

            _parent = null;
            _written.Clear();
            _taken.Clear();
            _cache.Push(this);
        }

        public void AbortToRoot()
        {
            Debug.Assert(_parent != null);
            foreach (var takenTuple in _taken)
            {
                if (takenTuple.Table.HierarchyLevel == 0)
                    takenTuple.Abort();
            }
            if (_parent.HierarchyLevel != 0)
                _parent.AbortToRoot();

            _parent = null;
            _written.Clear();
            _taken.Clear();
            _cache.Push(this);
        }
    }
}