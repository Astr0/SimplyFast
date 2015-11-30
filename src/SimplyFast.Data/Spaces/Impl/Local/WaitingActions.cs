using System;
using System.Collections.Generic;

namespace SF.Data.Spaces
{
    internal class WaitingActions<T>
    {
        private const int WaitingListCapacity = 1;

        private readonly Dictionary<int, List<WaitingAction>> _actions = new Dictionary<int,List<WaitingAction>>();
        private readonly List<WaitingAction> _globalActions = new List<WaitingAction>(32); 

        
        public void Cleanup(int transactionId)
        {
            _actions.Remove(transactionId);
        }

        public IDisposable AddGlobal(IQuery<T> query, Action<T> callback, bool take)
        {
            var action = new WaitingAction(query, callback, take);
            _globalActions.Add(action);
            return DisposableEx.Remove(_globalActions, action);
        }

        public IDisposable Add(int transactionId, IQuery<T> query, Action<T> callback, bool take)
        {
            List<WaitingAction> actions;
            if (!_actions.TryGetValue(transactionId, out actions))
            {
                actions = new List<WaitingAction>(WaitingListCapacity);
                _actions.Add(transactionId, actions);
            }
            var action = new WaitingAction(query, callback, take);
            actions.Add(action);
            return DisposableEx.Remove(actions, action);
        }

        public bool Taken(T tuple, int transactionId)
        {
            List<WaitingAction> actions;
            if(!_actions.TryGetValue(transactionId, out actions))
                return false;
            var taken = Taken(tuple, actions);
            if (actions.Count == 0)
                _actions.Remove(transactionId);
            return taken;
        }

        public bool Taken(T tuple)
        {
            var taken = Taken(tuple, _globalActions);
            foreach (var actions in _actions.Values)
            {
                taken = Taken(tuple, actions, taken);
            }
            return taken;
        }

        private static bool Taken(T tuple, List<WaitingAction> actions, bool taken = false)
        {
            // we should invoke read for everything, but take for only one action
            for (var i = actions.Count - 1; i >= 0; i--)
            {
                var action = actions[i];
                if (action.Take && taken)
                    continue;
                if (!action.Query.Match(tuple))
                    continue;
                action.Callback(tuple);
                actions.RemoveAt(i);
                if (action.Take)
                    taken = true;
            }
            return taken;
        }

        private class WaitingAction
        {
            public readonly IQuery<T> Query;
            public readonly Action<T> Callback;
            public readonly bool Take;

            public WaitingAction(IQuery<T> query, Action<T> callback, bool take)
            {
                Query = query;
                Callback = callback;
                Take = take;
            }
        }
    }
}