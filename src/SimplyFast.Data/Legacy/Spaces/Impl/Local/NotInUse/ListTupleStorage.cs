using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SF.Data.Legacy.Spaces.NotInUse
{
    internal class ListTupleStorage<T> : IEnumerable<T>, ITupleStorage<T>
        where T: class
    {
        private readonly List<T> _storage;

        public ListTupleStorage(int capacity)
        {
            _storage = new List<T>(capacity);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T tuple)
        {
            _storage.Add(tuple);
        }

        public T Read(IQuery<T> query)
        {
            return _storage.Find(query.Match);
        }

        public T Take(IQuery<T> query)
        {
            var index = _storage.FindIndex(query.Match);
            if (index < 0)
                return null;
            var res = _storage[index];
            _storage[index] = _storage[_storage.Count - 1];
            _storage.RemoveAt(_storage.Count - 1);
            return res;
        }
        
        public void Scan(IQuery<T> query, Action<T> callback)
        {
            foreach (var item in _storage)
            {
                if (query.Match(item))
                    callback(item);
            }
        }

        public int Count(IQuery<T> query)
        {
            return _storage.Count(query.Match);
        }

        public void Clear()
        {
            _storage.Clear();
        }

        public T[] GetArray(out int count)
        {
            count = _storage.Count;
            return _storage.ToArray();
        }

        public void AddRange(T[] tuples, int count)
        {
            _storage.AddRange(tuples.Take(count));
        }
    }
}