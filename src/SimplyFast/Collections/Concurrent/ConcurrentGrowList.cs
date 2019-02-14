using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

#pragma warning disable 420

namespace SimplyFast.Collections.Concurrent
{
    public class ConcurrentGrowList<T> : IReadOnlyList<T>
    {
        private volatile T[] _array;
        private volatile int _count;
        private volatile int _written;
        private volatile int _length;
        private int _lockTaken;

        public ConcurrentGrowList()
        {
            _array = TypeHelper<T>.EmptyArray;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public ConcurrentGrowList(int capacity)
        {
            _array = new T[capacity];
            _length = capacity;
        }

        public int Add(T item)
        {
            // declare that we will write new stuff
            var count = Interlocked.Increment(ref _count);

            // capture current array
            var arr = GetArray(count);

            // we can write something
            arr[count - 1] = item;
            Interlocked.Increment(ref _written);
            return count - 1;
        }

        private T[] GetArray(int count)
        {
            if (_length >= count)
                return _array;
            // ok, we don't have required length =\
            if (Interlocked.CompareExchange(ref _lockTaken, 1, 0) == 0)
                return LockResize(count);
            var spin = new SpinWait();
            while (true)
            {
                spin.SpinOnce();
                if (_length >= count)
                    return _array;
                if (Interlocked.CompareExchange(ref _lockTaken, 1, 0) == 0)
                    return LockResize(count);
            }
        }

        private T[] LockResize(int count)
        {
            // we're in lock, do something
            try
            {
                // someone resized out array, it's fine. 
                var len = _length;
                if (len >= count)
                    return _array;

                // hm... still have to resize
                WaitForWritten(len);
                var arr = _array;
                var cnt = _count;
                var newLength = cnt < 4 ? 4 : cnt * 2;
                Array.Resize(ref arr, newLength);
                _array = arr;
                _length = newLength;
                return arr;
            }
            finally
            {
                if (Interlocked.Exchange(ref _lockTaken, 0) == 0)
                    throw new InvalidOperationException("Shouldn't be here");
            }
        }

        private void WaitForWritten(int count)
        {
            if (_written >= count)
                return;
            var spin = new SpinWait();
            while (_written < count)
                spin.SpinOnce();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            var snapshot = new Snapshot(this);
            return snapshot.GetEnumerator();
        }

        public Snapshot GetSnapshot()
        {
            return new Snapshot(this);
        }

        public int Count => _count;

        public int Capacity => _length;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                    throw new IndexOutOfRangeException();
                WaitForWritten(index + 1);
                return _array[index];
            }
        }

        public struct Snapshot : IReadOnlyList<T>
        {
            private readonly int _count;
            private readonly T[] _arrayState;

            public Snapshot(ConcurrentGrowList<T> list)
            {
                var count = list._count;
                list.WaitForWritten(count);
                _arrayState = list._array;
                _count = count;
            }

            public IEnumerator<T> GetEnumerator()
            {
                var count = _count;
                for (var i = 0; i < count; i++)
                {
                    yield return _arrayState[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count => _count;

            public T this[int index]
            {
                get
                {
                    if (index < 0 || index >= _count)
                        throw new IndexOutOfRangeException();
                    return _arrayState[index];
                }
            }
        }
    }
}