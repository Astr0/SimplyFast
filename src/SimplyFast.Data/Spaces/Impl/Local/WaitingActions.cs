using System;
using System.Collections.Generic;

namespace SF.Data.Spaces
{
    internal class WaitingActions<T> where T : class
    {
        private const int WaitingListCapacity = 1;
        private readonly Dictionary<int, List<WaitingAction>> _actions = new Dictionary<int, List<WaitingAction>>();
        private readonly List<WaitingAction> _globalActions = new List<WaitingAction>(32);

        public void Cleanup(int transactionId)
        {
            _actions.Remove(transactionId);
        }

        public void AddGlobal(IQuery<T> query, Action<T> callback, TimeSpan timeout, bool take)
        {
            var action = new WaitingAction(query, callback, timeout, take);
            _globalActions.Add(action);
        }

        public void Add(int transactionId, IQuery<T> query, Action<T> callback, TimeSpan timeout, bool take)
        {
            List<WaitingAction> actions;
            if (!_actions.TryGetValue(transactionId, out actions))
            {
                actions = new List<WaitingAction>(WaitingListCapacity);
                _actions.Add(transactionId, actions);
            }
            var action = new WaitingAction(query, callback, timeout, take);
            actions.Add(action);
        }

        public bool Taken(T tuple, int transactionId, bool taken)
        {
            List<WaitingAction> actions;
            if (!_actions.TryGetValue(transactionId, out actions))
                return false;
            taken = Taken(tuple, actions, taken);
            if (actions.Count == 0)
                _actions.Remove(transactionId);
            return taken;
        }

        public bool Taken(T tuple)
        {
            var taken = Taken(tuple, _globalActions);
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var actions in _actions.Values)
            {
                taken = Taken(tuple, actions, taken);
            }
            return taken;
        }

        private static bool Taken(T tuple, List<WaitingAction> actions, bool taken = false)
        {
            var now = DateTime.UtcNow;
            // we should invoke read for everything, but take for only one action
            for (var i = actions.Count - 1; i >= 0; i--)
            {
                var action = actions[i];
                // check expiration
                if (action.Expire < now)
                {
                    action.Callback(null);
                    actions.RemoveAt(i);
                    continue;
                }
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
            public readonly Action<T> Callback;
            public readonly DateTime Expire;
            public readonly IQuery<T> Query;
            public readonly bool Take;

            public WaitingAction(IQuery<T> query, Action<T> callback, TimeSpan timeout, bool take)
            {
                Query = query;
                Callback = callback;
                Take = take;
                Expire = DateTime.UtcNow.Add(timeout);
            }
        }
    }
}