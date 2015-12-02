using System;

namespace SF.Data.Spaces
{
    public class ArrayTupleStorage<T>: ITupleStorage<T>
        where T:class
    {
        private T[] _storage;
        private int _count;

        public ArrayTupleStorage(int capacity)
        {
            _storage = new T[capacity];
        }

        public void Add(T tuple)
        {
            if (_count == _storage.Length)
            {
                Array.Resize(ref _storage, _count * 2);
            }
            _storage[_count++] = tuple;
        }

        public T Read(IQuery<T> query)
        {
            for (var i = 0; i < _count; i++)
            {
                var item = _storage[i];
                if (query.Match(item))
                    return item;
            }
            return null;
        }

        public T Take(IQuery<T> query)
        {
            for (var i = 0; i < _count; i++)
            {
                var item = _storage[i];
                if (!query.Match(item))
                    continue;
                // move last item to it's place
                _count--;
                _storage[i] = _storage[_count];
                _storage[_count] = null;
                return item;
            }
            return null;
        }

        public void Scan(IQuery<T> query, Action<T> callback)
        {
            for (var i = 0; i < _count; i++)
            {
                var item = _storage[i];
                if (query.Match(item))
                    callback(item);
            }
        }

        public int Count(IQuery<T> query)
        {
            var c = 0;
            for (var i = 0; i < _count; i++)
            {
                var item = _storage[i];
                if (item != null && query.Match(item))
                    c++;
            }
            return c;
        }

        public void Clear()
        {
            Array.Clear(_storage, 0, _count);
            _count = 0;
        }

        public T[] GetArray()
        {
            var res = new T[_count];
            Array.Copy(_storage, res, _count);
            return res;
        }
    }
}