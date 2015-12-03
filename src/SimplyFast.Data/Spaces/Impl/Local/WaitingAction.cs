using System;
using System.Collections.Generic;

namespace SF.Data.Spaces.Local
{
    internal class WaitingAction<T>
    {
        #region Cache

        private static int _nextId;
        private static readonly Stack<WaitingAction<T>> _pool = new Stack<WaitingAction<T>>(LocalSpaceConsts.WaitingActionsPoolCapacity);

        public static IDisposable Install(LocalTable<T> owner, IQuery<T> query, Action<T> callback, bool take)
        {
            var action = _pool.Count != 0 ? _pool.Pop() : new WaitingAction<T>(_nextId++);
            action._owner = owner;
            action._query = query;
            action._callback = callback;
            action._take = take;
            var root = owner.Root;
            var ownerWa = owner.WaitingAction;
            if (root != null)
            {
                // setup in owner 
                if (ownerWa != null)
                {
                    action._next = ownerWa;
                    ownerWa._prev = action;
                }
                owner.WaitingAction = action;

                // and it's root
                var rootWa = root.WaitingAction;
                if (rootWa != null)
                {
                    // setup in owner 
                    action._nextRoot = rootWa;
                    rootWa._prevRoot = action;
                }
                root.WaitingAction = action;
            }
            {
                // setup only in owner, but rooty
                if (ownerWa != null)
                {
                    action._nextRoot = ownerWa;
                    ownerWa._prevRoot = action;
                }
                owner.WaitingAction = action;
            }
            return action._disposable = new WaitingActionDisposable(action);
        }

        #endregion

        private WaitingAction(int id)
        {
            _id = id;
        }

        private readonly int _id;

        private LocalTable<T> _owner;
        private Action<T> _callback;
        private IQuery<T> _query;
        private bool _take;
        private WaitingActionDisposable _disposable;

        private class WaitingActionDisposable : IDisposable
        {
            private readonly WaitingAction<T> _action;
            public bool Disposed;

            public WaitingActionDisposable(WaitingAction<T> action)
            {
                _action = action;
            }

            public void Dispose()
            {
                if (Disposed)
                    return;
                // TODO: Something
            }
        }

        #region Crazy double-double-linked list stuff

        private WaitingAction<T> _prev;
        private WaitingAction<T> _next;
        private WaitingAction<T> _prevRoot;
        private WaitingAction<T> _nextRoot;

        #endregion

        public bool Take(T tuple)
        {
            var wa = this;
            do
            {
                if (wa._query.Match(tuple))
                {
                    var a = wa;
                    wa = wa._next;
                    a._callback(tuple);
                    Remove(a);
                    if (a._take)
                        return true;
                }
                {
                    wa = wa._next;
                }

            } while (wa != null);
            return false;
        }

        public bool TakeRoot(T tuple)
        {
            var wa = this;
            do
            {
                if (wa._query.Match(tuple))
                {
                    var a = wa;
                    wa = wa._nextRoot;
                    a._callback(tuple);
                    Remove(a);
                    if (a._take)
                        return true;
                }
                {
                    wa = wa._next;
                }

            } while (wa != null);
            return false;
        }

        private static void Remove(WaitingAction<T> wa)
        {
            wa._disposable.Disposed = true;
            wa._query = null;
            wa._callback = null;

            // cleanup rooty list
            if (wa._prevRoot == null)
            {
                // this is head, so set new haed
                if (wa._owner.Root != null)
                    wa._owner.Root.WaitingAction = wa._nextRoot;
                else
                    wa._owner.WaitingAction = wa._nextRoot;

                if (wa._nextRoot == null)
                    return;
                // set next's prev to null
                wa._nextRoot._prevRoot = null;

                // cleanup self
                wa._nextRoot = null;
            }
            else
            {
                // set prev's next
                wa._prevRoot._nextRoot = wa._nextRoot;
                if (wa._nextRoot != null)
                {
                    // set next's prev
                    wa._nextRoot._prevRoot = wa._prevRoot;
                    wa._nextRoot = null;
                }
                wa._prevRoot = null;
            }

            // if root is owner, then those stuff was not set during Install
            if (wa._owner.Root == null)
                return;

            // cleanup normal list
            if (wa._prev == null)
            {
                // this is head, so set new haed
                wa._owner.WaitingAction = wa._next;
                if (wa._next == null)
                    return;
                // set next's prev to null
                wa._next._prev = null;

                // cleanup self
                wa._next = null;
            }
            else
            {
                // set prev's next
                wa._prev._next = wa._next;
                if (wa._next != null)
                {
                    // set next's prev
                    wa._next._prev = wa._prev;
                    wa._next = null;
                }
                wa._prev = null;
            }
        }


        public int Take(T[] tuples, int count)
        {
            throw new NotImplementedException();
        }

        public int TakeRoot(T[] tuples, int count)
        {
            throw new NotImplementedException();
        }
    }
}