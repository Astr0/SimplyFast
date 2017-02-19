using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SF.Collections
{
    public class FastCollection<T> : ICollection<T>
    {
        private T[] _array;
        private int _count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastCollection(int capacity)
        {
            _array = new T[Math.Max(4, capacity)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastCollection()
        {
            _array = new T[4];
        }

        public T this[int index]
        {
            get { return _array[index]; }
            set { _array[index] = value; }
        }

        public T[] Buffer => _array;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
        {
            return _array.Take(_count).GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (_array.Length == _count)
                Array.Resize(ref _array, _count * 2);
            _array[_count++] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_array, 0, array, arrayIndex, _count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index < 0)
                return false;
            RemoveAt(index);
            return true;
        }

        public int Count => _count;

        public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item)
        {
            return Array.IndexOf(_array, item, 0, _count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int i)
        {
            _count--;
            _array[i] = _array[_count];
        }

        public void AddRange(T[] items, int count)
        {
            if (_count + count > _array.Length)
            {
                var newCount = Math.Max(_array.Length * 2, _count + count);
                var arr = new T[newCount];
                Array.Copy(_array, arr, _count);
                _array = arr;
            }

            Array.Copy(items, 0, _array, _count, count);
            _count += count;
        }
    }
}