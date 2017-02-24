using System;
using System.Collections.Generic;
using SimplyFast.Collections;
using SimplyFast.Data.Spaces.Interface;

namespace SimplyFast.Data.Spaces.Impl.Local
{
    internal interface IWaitingAction
    {
        void Remove();
    }

    internal static class WaitingAction
    {
        public static void RemoveAll(LinkedList<IWaitingAction> waitingActions)
        {
            var node = waitingActions.First;
            while (node != null)
            {
                var wa = node.Value;
                node = node.Next;
                wa.Remove();
            }
        }
    }

    internal class WaitingAction<T>: IWaitingAction
    {
        private static readonly FastStack<WaitingAction<T>> _pool = new FastStack<WaitingAction<T>>(LocalSpaceConsts.WaitingActionsPoolCapacity);

        public static IDisposable Install(LocalTable<T> owner, LinkedList<IWaitingAction> root, IQuery<T> query, Action<T> callback, bool take)
        {
            var action = _pool.Count != 0 ? _pool.Pop() : new WaitingAction<T>();
            action._owner = owner;
            action._query = query;
            action._callback = callback;
            action._take = take;
            // setup in owner 
            owner.WaitingActions.AddLast(action._ownerNode);
            root.AddLast(action._rootNode);
            // recreate disposable, so recycling actions wont be removed by exising disposables
            return action._disposable = new WaitingActionDisposable(action);
        }
        
        private WaitingAction()
        {
            _ownerNode = new LinkedListNode<IWaitingAction>(this);
            _rootNode = new LinkedListNode<IWaitingAction>(this);
        }

        private LocalTable<T> _owner;
        private Action<T> _callback;
        private IQuery<T> _query;
        private bool _take;
        private WaitingActionDisposable _disposable;
        private readonly LinkedListNode<IWaitingAction> _ownerNode;
        private readonly LinkedListNode<IWaitingAction> _rootNode;

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
                _action.Remove();
            }
        }

        public static bool Take(LocalTable<T> table, T tuple)
        {
            var node = table.WaitingActions.First;
            do
            {
                var wa = (WaitingAction<T>)node.Value;
                node = node.Next;
                if (!wa._query.Match(tuple))
                    continue;
                if (wa._take)
                {
                    if (table != wa._owner)
                        wa._owner.AddTaken(table, tuple);
                    wa.Remove(tuple);
                    return true;
                }
                wa.Remove(tuple);
            } while (node != null);
            return false;
        }


        public static int Take(LocalTable<T> table, T[] tuples, int count)
        {
            var node = table.WaitingActions.First;
            var taken = 0;
            do
            {
                var wa = (WaitingAction<T>)node.Value;
                node = node.Next;
                var i = 0;
                while (i < count)
                {
                    var tuple = tuples[i];
                    if (!wa._query.Match(tuple))
                    {
                        ++i;
                        continue;
                    }
                    if (wa._take)
                    {
                        if (table != wa._owner)
                            wa._owner.AddTaken(table, tuple);
                        wa.Remove(tuple);

                        count--;
                        taken++;
                        // switch tuple with last tuple so taken tuples are at the end
                        tuples[i] = tuples[count];
                        tuples[count] = tuple;
                        if (count == 0)
                            return taken;
                    }
                    else
                    {
                        wa.Remove(tuple);
                    }
                    break;
                }
            } while (node != null);
            return taken;
        }

        public void Remove()
        {
            Remove(default(T));
        }

        private void Remove(T tuple)
        {
            _disposable.Disposed = true;
            _query = null;
            _callback(tuple);
            _callback = null;

            // cleanup lists
            _ownerNode.List.Remove(_ownerNode);
            _rootNode.List.Remove(_rootNode);

            // recycle itself
            _pool.Push(this);
        }
    }
}