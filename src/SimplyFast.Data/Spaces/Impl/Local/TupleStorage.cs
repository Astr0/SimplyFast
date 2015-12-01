using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SF.Data.Spaces
{
    internal class TupleStorage<T> : IEnumerable<T>
    {
        private readonly ICollection<T> _storage;

        public TupleStorage() : this(new List<T>())
        {
        }

        public TupleStorage(ICollection<T> storage)
        {
            _storage = storage;
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
            return _storage.FirstOrDefault(query.Match);
        }

        public T Take(IQuery<T> query)
        {
            var result = Read(query);
            if (result != null)
                _storage.Remove(result);
            return result;
        }

        public IEnumerable<T> Scan(IQuery<T> query)
        {
            return _storage.Where(query.Match);
        }

        public int Count(IQuery<T> query)
        {
            return _storage.Count(query.Match);
        }

        public void Clear()
        {
            _storage.Clear();
        }
    }
}