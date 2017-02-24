using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SimplyFast.IO
{
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class FastBufferReader
    {
        #region local var int consts to make code more readable
        private const byte VarIntData = VarIntHelper.VarIntDataMask;
        private const byte VarIntMore = VarIntHelper.VarIntMoreFlag;
        private const int VarIntDataBits = VarIntHelper.VarIntDataBits;
        private const int VarIntDataBits2 = 2 *  VarIntHelper.VarIntDataBits;
        private const int VarIntDataBits3 = 3 * VarIntHelper.VarIntDataBits;
        private const int VarIntDataBits4 = 4 * VarIntHelper.VarIntDataBits;

        private const uint VarIntData32 = VarIntData;
        private const uint VarIntMore32 = VarIntMore;

        //private const ulong VarIntData64 = VarIntData;
        //private const ulong VarIntMore64 = VarIntMore;
        #endregion

        private readonly byte[] _buffer;
        private int _index;
        private int _length;

        public FastBufferReader(byte[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            _length = index + count;
            if (_length > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "Buffer index + count > Length");
            _buffer = buffer;
            _index = index;
        }

        public FastBufferReader(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            _buffer = buffer;
            _length = buffer.Length;
        }

        public void RestoreView(BufferView view)
        {
            if (_index > view.Length)
                throw new InvalidOperationException("Can't restore view. Already read data after end of view.");
            _length = view.Length;
        }

        public BufferView SetView(int length)
        {
            // can't set more than we have now
            var end = _index + length;
            if (end > _length)
                throw OutOfInputException();
            var view = new BufferView(_length);
            _length = end;
            return view;
        }

        public bool End => _index == _length;

        public void SkipBytes(int count)
        {
            _index += count;
            if (_index > _length)
                throw OutOfInputException();
        }

        public double ReadDouble()
        {
            return BitConverter.Int64BitsToDouble((long)ReadLittleEndian64());
        }

        public float ReadLittleEndianFloat()
        {
            var i = _index;
            if (i + 4 > _length)
                throw OutOfInputException();
            if (BitConverter.IsLittleEndian)
            {
                var res = BitConverter.ToSingle(_buffer, i);
                _index = i + 4;
                return res;
            }

            var bytes = UncheckedReadRawBytes(4);
            ReverseBytes(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static void ReverseBytes(byte[] bytes)
        {
            var start = 0;
            // start reverse so JIT may have chance to optimize array access checks
            for (var end = bytes.Length - 1; start < end; --end)
            {
                var tmp = bytes[start];
                bytes[start] = bytes[end];
                bytes[end] = tmp;
                ++start;
            }
        }


        public uint ReadLittleEndian32()
        {
            var i = _index;
            if (i + 4 > _length)
                throw OutOfInputException();
            var buf = _buffer;
            uint b0 = buf[i];
            uint b1 = buf[i+1];
            uint b2 = buf[i+2];
            uint b3 = buf[i+3];
            _index = i + 4;
            return b0 | b1 << 8 | b2 << 16 | b3 << 24;
        }

        public ulong ReadLittleEndian64()
        {
            var i = _index;
            if (i + 8 > _length)
                throw OutOfInputException();
            var buf = _buffer;
            ulong b0 = buf[i];
            ulong b1 = buf[i + 1];
            ulong b2 = buf[i + 2];
            ulong b3 = buf[i + 3];
            ulong b4 = buf[i + 4];
            ulong b5 = buf[i + 5];
            ulong b6 = buf[i + 6];
            ulong b7 = buf[i + 7];
            _index = i + 8;
            return b0 | b1 << 8 | b2 << 16 | b3 << 24 | b4 << 32 | b5 << 40 | b6 << 48 | b7 << 56;
        }

        public void SkipVarInt()
        {
            var i = _index;
            var end = i + 10;
            var len = _length;
            if (end > len)
                end = len;
            // fast skip
            var buf = _buffer;
            while (i < end)
            {
                var readByte = buf[i++];
                // ReSharper disable once InvertIf
                if (readByte < VarIntMore)
                {
                    _index = i;
                    return;
                }
            }
            // check what to throw
            if (i == len)
                throw OutOfInputException();
            _index = i;
            throw InvalidVarInt64Exception();
            
        }

        public ulong ReadVarInt64()
        {
            var shift = 0;
            ulong value = 0;
            var i = _index;
            var len = _length;
            var end = i + 10;
            if (end > len)
                end = len;

            var buf = _buffer;
            while (i < end)
            {
                var readByte = buf[i++];
                value |= (ulong)(readByte & VarIntData) << shift;
                if (readByte < VarIntMore)
                {
                    _index = i;
                    return value;
                }
                shift += VarIntDataBits;
            }
            // check what to throw
            if (i == len)
            {
                throw OutOfInputException();
            }
            _index = i;
            throw InvalidVarInt64Exception();
        }

        public uint ReadVarInt32()
        {
            uint value;
            var i = _index;
            if (i + 5 <= _length)
            {
                var buf = _buffer;
                // 1
                uint b0 = buf[i];
                if (b0 < VarIntMore32)
                {
                    _index = i + 1;
                    return b0;
                }
                var v0 = b0 & VarIntData32;

                // 2
                uint b1 = buf[i + 1];
                if (b1 < VarIntMore32)
                {
                    _index = i + 2;
                    return v0 | b1 << VarIntDataBits;
                }
                var v1 = v0 | (b1 & VarIntData32) << VarIntDataBits;

                // 3
                uint b2 = buf[i + 2];
                if (b2 < VarIntMore32)
                {
                    _index = i + 3;
                    return v1 | b2 << VarIntDataBits2;
                }
                var v2 = v1 | (b2 & VarIntData32) << VarIntDataBits2;

                // 4
                uint b3 = buf[i + 3];
                if (b3 < VarIntMore32)
                {
                    _index = i + 4;
                    return v2 | b3 << VarIntDataBits3;
                }

                // 5
                uint b4 = buf[i + 4];
                _index = i + 5;
                value = v2 | (b3 & VarIntData32) << VarIntDataBits3 | b4 << VarIntDataBits4;
                if (b4 < VarIntMore32)
                    return value;
            }
            else
            {
                // we almost at the end of stream, read one by one (same code)

                // 1
                uint b0 = ReadByte();
                if (b0 < VarIntMore32)
                    return b0;
                var v0 = b0 & VarIntData32;
                
                // 2
                uint b1 = ReadByte();
                if (b1 < VarIntMore32)
                {
                    return v0 | b1 << VarIntDataBits;
                }
                var v1 = v0 | (b1 & VarIntData32) << VarIntDataBits;

                // 3
                uint b2 = ReadByte();
                if (b2 < VarIntMore32)
                {
                    return v1 | b2 << VarIntDataBits2;
                }
                var v2 = v1 | (b2 & VarIntData32) << VarIntDataBits2;

                // 4
                uint b3 = ReadByte();
                if (b3 < VarIntMore32)
                {
                    return v2 | b3 << VarIntDataBits3;
                }

                // 5
                uint b4 = ReadByte();
                value = v2 | (b3 & VarIntData32) << VarIntDataBits3 | b4 << VarIntDataBits4;
                if (b4 < VarIntMore32)
                    return value;
            }

            // we here, so we have some Variant64... discard the large bits
            //6
            if (ReadByte() < VarIntMore)
                return value;
            //7
            if (ReadByte() < VarIntMore)
                return value;
            //8
            if (ReadByte() < VarIntMore)
                return value;
            //9
            if (ReadByte() < VarIntMore)
                return value;
            //10
            if (ReadByte() < VarIntMore)
                return value;

            throw InvalidVarInt32Exception();
        }

        public byte ReadByte()
        {
            if (_index == _length)
                throw OutOfInputException();
            return _buffer[_index++];
        }

        public byte[] ReadRawBytes(int length)
        {
            var i = _index;
            if (i + length > _length)
                throw OutOfInputException();
            var bytes = new byte[length];
            Array.Copy(_buffer, i, bytes, 0, length);
            _index = i + length;
            return bytes;
        }

        private byte[] UncheckedReadRawBytes(int length)
        {
            var bytes = new byte[length];
            Array.Copy(_buffer, _index, bytes, 0, length);
            _index += length;
            return bytes;
        }

        public string ReadRawUtf8String(int length)
        {
            var i = _index;
            if (i + length > _length)
                throw OutOfInputException();
            var res = FastBufferInternal.Utf8Encoding.GetString(_buffer, i, length);
            _index = i + length;
            return res;
        }

        private static InvalidDataException InvalidVarInt32Exception()
        {
            return new InvalidDataException("Invalid VarInt32 format");
        }

        private static InvalidDataException InvalidVarInt64Exception()
        {
            return new InvalidDataException("Invalid VarInt64 format");
        }

        protected InvalidDataException OutOfInputException()
        {
            // avoid further reading from ended stream
            _index = _length;
            return new InvalidDataException("Read attempt beyond end of buffer");
        }

        public struct BufferView
        {
            internal BufferView(int length)
            {
                Length = length;
            }

            internal readonly int Length;
        }
    }
}