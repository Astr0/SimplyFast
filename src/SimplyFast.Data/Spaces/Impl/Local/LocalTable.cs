using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SimplyFast.Collections;
using SimplyFast.Data.Spaces.Impl.Local.Storage;
using SimplyFast.Data.Spaces.Interface;
using SimplyFast.Disposables;

namespace SimplyFast.Data.Spaces.Impl.Local
{
    internal class LocalTable<T>: ILocalTable
    {
        private readonly ITupleStorage<T> _written;
        private readonly FastCollection<TakenTuple<T>> _taken = new FastCollection<TakenTuple<T>>(LocalSpaceConsts.TransactionTakenCapacity);
        //private TakenTuple<T>[] _taken = new TakenTuple<T>[LocalSpaceConsts.TransactionTakenCapacity];
        //private int _takenCount;

        private LocalTable<T> _parent;
        public LocalTable<T> Root;
        public readonly LinkedList<IWaitingAction> WaitingActions = new LinkedList<IWaitingAction>(); 

        #region Factory

        public static LocalTable<T> GetRoot()
        {
            return new LocalTable<T>(new ArrayTupleStorage<T>(LocalSpaceConsts.RootWrittenCapacity));
        }

        private static readonly FastStack<LocalTable<T>> _cache = new FastStack<LocalTable<T>>(LocalSpaceConsts.TransactionCacheCapacity);
        public static LocalTable<T> GetTransactional(int hierarchyLevel, LocalTable<T> parent)
        {
            var trans = _cache.Count != 0 ? _cache.Pop() : new LocalTable<T>(new ArrayTupleStorage<T>(LocalSpaceConsts.TransactionWrittenCapacity));
            trans.HierarchyLevel = hierarchyLevel;
            trans._parent = parent;
            trans.Root = parent.Root ?? parent;
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
                    AddTaken(borrower, taken);
                    return taken;
                }
                borrower = borrower._parent;
            }
            return default(T);
        }

        public IDisposable Read(IQuery<T> query, Action<T> callback, LinkedList<IWaitingAction> ownerList)
        {
            var readItem = TryRead(query);
            if (readItem != null)
            {
                callback(readItem);
                return DisposableEx.Null();
            }

            // add waiting action
            return WaitingAction<T>.Install(this, Root != null ? Root.WaitingActions : ownerList, query, callback, false);
        }

        public IDisposable Take(IQuery<T> query, Action<T> callback, LinkedList<IWaitingAction> ownerList)
        {
            var takenItem = TryTake(query);
            if (takenItem != null)
            {
                callback(takenItem);
                return DisposableEx.Null();
            }

            // add waiting action
            return WaitingAction<T>.Install(this, Root != null ? Root.WaitingActions : ownerList, query, callback, true);
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
            if (WaitingActions.First != null && WaitingAction<T>.Take(this, tuple))
                return;
            _written.Add(tuple);
        }

        private void AddRange(T[] tuples, int count)
        {
            if (count == 0)
                return;
            if (WaitingActions.First == null)
                _written.AddRange(tuples, count);
            else
                _written.AddRange(tuples, count - (WaitingAction<T>.Take(this, tuples, count)));
        }

        public void AddRange(T[] tuples)
        {
            AddRange(tuples, tuples.Length);
        }

        public ILocalTable Parent => _parent;
        public int HierarchyLevel { get; private set; }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Cleanup()
        {
            //_parent = null;
            _written.Clear();
            // TODO: do we need to clear the array
            //if (_takenCount != 0)
            //{
            //    Array.Clear(_taken, 0, _takenCount);
            //    _takenCount = 0;
            //}
            _taken.Clear();
            _cache.Push(this);
        }

        public void Commit()
        {
            Debug.Assert(_parent != null);
            WaitingAction.RemoveAll(WaitingActions);

            int count;
            var written = _written.GetArray(out count);
            _parent.AddRange(written, count);

            Cleanup();
        }



        public void Abort()
        {
            Debug.Assert(_parent != null);
            WaitingAction.RemoveAll(WaitingActions);

            for (var i = 0; i < _taken.Count; i++)
            {
                _taken[i].Abort();
            }

            Cleanup();
        }

        public void AbortToRoot()
        {
            Debug.Assert(_parent != null);
            WaitingAction.RemoveAll(WaitingActions);

            for (var i = 0; i < _taken.Count; i++)
            {
                _taken[i].AbortToRoot();
            }

            if (_parent.HierarchyLevel != 0)
                _parent.AbortToRoot();

            Cleanup();
        }

        public void AddTaken(LocalTable<T> borrower, T tuple)
        {
            /*if (_takenCount == _taken.Length)
            {
                Array.Resize(ref _taken, _takenCount * 2);
            }
            _taken[_takenCount++] = new TakenTuple<T>(borrower, tuple);*/
            _taken.Add(new TakenTuple<T>(borrower, tuple));
        }
    }
}