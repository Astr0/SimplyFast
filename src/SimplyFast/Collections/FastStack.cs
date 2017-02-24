using System;
using System.Runtime.CompilerServices;

namespace SimplyFast.Collections
{
    public class FastStack<T>
    {
        private T[] _array;
        private int _count;

        public FastStack()
        {
            _array = TypeHelper<T>.EmptyArray;
        }

        public FastStack(int capacity)
        {
            _array = capacity > 0 ? new T[capacity] : TypeHelper<T>.EmptyArray;
        }

        public int Count => _count;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T value)
        {
            if (_count == _array.Length)
                Array.Resize(ref _array, _count != 0 ? _count * 2 : 4);
            _array[_count++] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek()
        {
            return _array[_count - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            var v = _array[--_count];
            if (TypeHelper<T>.IsReferenceType)
                _array[_count] = default(T);
            return v;
        }
    }
}