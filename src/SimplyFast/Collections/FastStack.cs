using System;
using System.Runtime.CompilerServices;

namespace SF.Collections
{
    public class FastStack<T>
    {
        private T[] _array;
        private int _count;

        public FastStack()
        {
            _array = new T[4];
        }

        public FastStack(int capacity)
        {
            _array = new T[Math.Max(4, capacity)];
        }

        public int Count => _count;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T value)
        {
            if (_count == _array.Length)
            {
                Array.Resize(ref _array, _count * 2);
            }
            _array[_count++] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            return _array[--_count];
        }
    }
}