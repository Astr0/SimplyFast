using System;

namespace SF.Data.Spaces
{
    internal class WaitingActions<T> where T : class
    {
        private const int WaitingListCapacity = 2;
        private WaitingAction[] _actions = new WaitingAction[WaitingListCapacity];
        private int _count;

        public void Add(IQuery<T> query, Action<T> callback, DateTime expire, bool take)
        {
            if (_count == _actions.Length)
            {
                Array.Resize(ref _actions, _count * 2);
            }
            _actions[_count++] = new WaitingAction(query, callback, expire, take);
        }

        public bool PendingTake(T tuple)
        {
            if (_count == 0)
                return false;

            var now = DateTime.UtcNow;
            var i = 0;
            while (i < _count)
            {
                var action = _actions[i];
                // check expiration
                if (action.Expire < now)
                {
                    action.Callback(null);
                    Remove(i);
                    continue;
                }
                if (!action.Query.Match(tuple))
                {
                    ++i;
                    continue;
                }
                action.Callback(tuple);
                Remove(i);
                if (action.Take)
                    return true;
            }
            return false;
        }

        private void Remove(int i)
        {
            --_count;
            _actions[i] = _actions[_count];
            _actions[_count] = null;
        }

        public int PendingTake(T[] tuples, int count)
        {
            if (_count == 0)
                return 0;

            var now = DateTime.UtcNow;
            var i = 0;
            // check expiration
            while (i < _count)
            {
                var action = _actions[i];
                if (action.Expire < now)
                {
                    action.Callback(null);
                    Remove(i);
                }
                else
                {
                    ++i;
                }
            }

            var taken = 0;
            var j = 0;
            while (j < count)
            {
                if (_count == 0)
                    break;
                var tuple = tuples[j];
                i = 0;
                while (i < _count)
                {
                    var action = _actions[i];

                    if (!action.Query.Match(tuple))
                    {
                        ++i;
                        continue;
                    }
                    action.Callback(tuple);
                    Remove(i);
                    if (action.Take)
                    {
                        // increase number of taken and place taken item to the end of tuples
                        ++taken;
                        --j;
                        --count;
                        tuples[j] = tuples[count];
                        tuples[count] = tuple;
                        break;
                    }
                }
                
                ++j;
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

        public void Clear()
        {
            if (_count == 0)
                return;
            Array.Clear(_actions, 0, _count);
            _count = 0;
        }

    }
}