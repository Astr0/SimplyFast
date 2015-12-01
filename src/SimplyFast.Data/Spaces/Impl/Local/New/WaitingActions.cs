using System;
using System.Collections.Generic;

namespace SF.Data.Spaces.New
{
    internal class WaitingActions<T> where T : class
    {
        private const int WaitingListCapacity = 1;
        private readonly List<WaitingAction> _actions = new List<WaitingAction>(WaitingListCapacity);

        public void Add(IQuery<T> query, Action<T> callback, DateTime expire, bool take)
        {
            var action = new WaitingAction(query, callback, expire, take);
            _actions.Add(action);
        }

        public bool Taken(T tuple, DateTime now, bool taken = false)
        {
            // we should invoke read for everything, but take for only one action
            for (var i = _actions.Count - 1; i >= 0; i--)
            {
                var action = _actions[i];
                // check expiration
                if (action.Expire < now)
                {
                    action.Callback(null);
                    _actions.RemoveAt(i);
                    continue;
                }
                if (action.Take && taken)
                    continue;
                if (!action.Query.Match(tuple))
                    continue;
                action.Callback(tuple);
                _actions.RemoveAt(i);
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