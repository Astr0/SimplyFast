using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SF.Collections
{
    public class FastCollection<T> : ICollection<T>, IReadOnlyList<T>
    {
        private T[] _array;
        private int _count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastCollection(int capacity)
        {
            _array = capacity > 0? new T[capacity] : TypeHelper<T>.EmptyArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastCollection()
        {
            _array = TypeHelper<T>.EmptyArray;
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
            for (var i = 0; i < _count; i++)
            {
                yield return _array[i];
            }
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
                Array.Resize(ref _array, _count == 0 ? 4 : _count * 2);
            _array[_count++] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            if (TypeHelper<T>.IsReferenceType)
                Array.Clear(_array, 0, _count);
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

        public int Capacity
        {
            get { return _array.Length; }
            set
            {
                if (value < _count)
                    throw new InvalidOperationException("Collection has more elements that requested capacity");
                var arr = new T[value];
                Array.Copy(_array, 0, arr, 0, _count);
                _array = arr;
            }
        }

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
            if (TypeHelper<T>.IsReferenceType)
                _array[_count] = default(T);
        }

        public void AddRange(T[] items, int count)
        {
            if (_count + count > _array.Length)
                Capacity = Math.Max(_array.Length * 2, _count + count);

            Array.Copy(items, 0, _array, _count, count);
            _count += count;
        }

        //public void Insert(int index, T item)
        //{
        //    var count = _count;
        //    if (index < 0 || index > count)
        //        throw new IndexOutOfRangeException();
        //    if (index == count)
        //    {
        //        Add(item);
        //    }
        //    else if (_array.Length == count)
        //    {
        //        var arr = new T[count * 2];
        //        if (index != 0)
        //            Array.Copy(_array, 0, arr, 0, index);
        //        Array.Copy(_array, index, arr, index + 1, count - index);
        //        arr[index] = item;
        //        _count = count + 1;
        //        _array = arr;
        //    }
        //    else
        //    {
        //        Array.Copy(_array, index, _array, index + 1, count - index);
        //        _array[index] = item;
        //        _count = count + 1;
        //    }
        //}
    }
}