using System;

namespace SF.Data.Spaces.Local
{
    internal class ArrayTupleStorage<T> : ITupleStorage<T>
    {
        private int _count;
        private T[] _storage;

        public ArrayTupleStorage(int capacity)
        {
            _storage = new T[capacity];
        }

        public void Add(T tuple)
        {
            if (_count == _storage.Length)
            {
                Array.Resize(ref _storage, _count*2);
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
            return default(T);
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
                _storage[_count] = default(T);
                return item;
            }
            return default(T);
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
            if (_count == 0)
                return;
            Array.Clear(_storage, 0, _count);
            _count = 0;
        }

        public T[] GetArray(out int count)
        {
            count = _count;
            return _storage;
        }

        public void AddRange(T[] tuples, int count)
        {
            if (_count + count > _storage.Length)
            {
                var newCount = Math.Max(_storage.Length*2, _count + count);
                Array.Resize(ref _storage, newCount);
            }

            for (var i = 0; i < count; i++)
            {
                _storage[_count + i] = tuples[i];
            }
            _count += count;
        }
    }
}