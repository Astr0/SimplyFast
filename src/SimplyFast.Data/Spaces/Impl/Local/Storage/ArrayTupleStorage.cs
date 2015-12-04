using System;
using SF.Collections;

namespace SF.Data.Spaces.Local
{
    internal class ArrayTupleStorage<T> : ITupleStorage<T>
    {
        private readonly FastCollection<T> _storage; 

        public ArrayTupleStorage(int capacity)
        {
            _storage = new FastCollection<T>(capacity);
        }

        public void Add(T tuple)
        {
            _storage.Add(tuple);
        }

        public T Read(IQuery<T> query)
        {
            for (var i = 0; i < _storage.Count; i++)
            {
                var item = _storage[i];
                if (query.Match(item))
                    return item;
            }
            return default(T);
        }

        public T Take(IQuery<T> query)
        {
            for (var i = 0; i < _storage.Count; i++)
            {
                var item = _storage[i];
                if (!query.Match(item))
                    continue;
                // move last item to it's place
                _storage.RemoveAt(i);
                return item;
            }
            return default(T);
        }

        public void Scan(IQuery<T> query, Action<T> callback)
        {
            for (var i = 0; i < _storage.Count; i++)
            {
                var item = _storage[i];
                if (query.Match(item))
                    callback(item);
            }
        }

        public int Count(IQuery<T> query)
        {
            var c = 0;
            for (var i = 0; i < _storage.Count; i++)
            {
                var item = _storage[i];
                if (item != null && query.Match(item))
                    c++;
            }
            return c;
        }

        public void Clear()
        {
            _storage.Clear();
        }

        public T[] GetArray(out int count)
        {
            count = _storage.Count;
            return _storage.Buffer;
        }

        public void AddRange(T[] tuples, int count)
        {
            _storage.AddRange(tuples, count);
        }
    }
}