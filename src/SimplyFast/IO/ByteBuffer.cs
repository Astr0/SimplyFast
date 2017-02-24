using System;
using System.Linq;
using SimplyFast.Collections;

namespace SimplyFast.IO
{
    public class ByteBuffer
    {
        private byte[] _buffer;
        private int _count;
        private int _offset;

        public ByteBuffer()
        {
            _buffer = TypeHelper<byte>.EmptyArray;
        }

        public ByteBuffer(byte[] buffer, int offset, int count)
        {
            _buffer = buffer ?? TypeHelper<byte>.EmptyArray;
            SetView(offset, count);
        }

        public byte[] Buffer => _buffer;
        public int Offset => _offset;
        public int Count => _count;
        public int BufferLength => _buffer.Length;

        public void SetView(int offset, int count)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (offset + count > _buffer.Length)
                throw new ArgumentOutOfRangeException(offset >= _buffer.Length ? nameof(offset) : nameof(count));
            _offset = offset;
            _count = count;
        }

        public void Reset(int length)
        {
            SetView(0, 0);
            if (_buffer.Length < length)
                _buffer = new byte[length];
        }

        public void Grow(int newSize = 0)
        {
            if (newSize == 0)
                newSize = Math.Max(4, BufferLength * 2);
            else if (newSize < BufferLength)
                throw new ArgumentOutOfRangeException(nameof(newSize), "Can't grow less than buffer size.");

            Array.Resize(ref _buffer, newSize);
        }

        public override string ToString()
        {
            return string.Join("_", _buffer.Skip(_offset).Take(_count));
        }
    }
}