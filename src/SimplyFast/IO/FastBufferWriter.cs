using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SimplyFast.IO
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public class FastBufferWriter
    {
        #region local var int consts to make code more readable
        // ReSharper disable InconsistentNaming
        private const byte VarIntData = VarIntHelper.VarIntDataMask;
        private const byte VarIntMore = VarIntHelper.VarIntMoreFlag;
        private const int VarIntDataBits = VarIntHelper.VarIntDataBits;

        private const uint VarIntData32 = VarIntData;
        private const uint VarIntMore32 = VarIntMore;

        private const ulong VarIntData64 = VarIntData;
        private const ulong VarIntMore64 = VarIntMore;
        // ReSharper restore InconsistentNaming
        #endregion


        private readonly byte[] _buffer;
        private int _index;

        public int Index => _index;

        private readonly int _startIndex;
        private readonly int _length;

        public FastBufferWriter(byte[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            _length = index + count;
            if (_length > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "Buffer index + count > Length");
            _buffer = buffer;
            _startIndex = index;
            _index = index;
        }

        public FastBufferWriter(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            _buffer = buffer;
            _length = buffer.Length;
        }

        public void WriteByte(byte b)
        {
            if (_index == _length)
                throw OutOfBufferException();
            _buffer[_index++] = b;
        }

        public void WriteBytes(byte b1, byte b2)
        {
            var i = _index;
            if (i + 2 > _length)
                throw OutOfBufferException();
            _buffer[i] = b1;
            _buffer[i + 1] = b2;
            _index = i + 2;
        }

        public void WriteBytes(byte b1, byte b2, byte b3)
        {
            var i = _index;
            if (i + 3 > _length)
                throw OutOfBufferException();
            var buf = _buffer;
            buf[i] = b1;
            buf[i + 1] = b2;
            buf[i + 2] = b3;
            _index = i + 3;
        }

        public void WriteBytes(byte b1, byte b2, byte b3, byte b4)
        {
            var i = _index;
            if (i + 4 > _length)
                throw OutOfBufferException();
            var buf = _buffer;
            buf[i] = b1;
            buf[i + 1] = b2;
            buf[i + 2] = b3;
            buf[i + 3] = b4;
            _index = i + 4;
        }

        public void WriteBytes(byte b1, byte b2, byte b3, byte b4, byte b5)
        {
            var i = _index;
            if (i + 5 > _length)
                throw OutOfBufferException();
            var buf = _buffer;
            buf[i] = b1;
            buf[i + 1] = b2;
            buf[i + 2] = b3;
            buf[i + 3] = b4;
            buf[i + 4] = b5;
            _index = i + 5;
        }

        public void WriteLittleEndian32(uint value)
        {
            var i = _index;
            if (i + 4 > _length)
                throw OutOfBufferException();
            var buf = _buffer;
            buf[i] = (byte)value;
            buf[i + 1] = (byte)(value >> 8);
            buf[i + 2] = (byte)(value >> 16);
            buf[i + 3] = (byte)(value >> 24);
            _index = i + 4;
        }

        public void WriteLittleEndian64(ulong value)
        {
            var i = _index;
            if (i + 8 > _length)
                throw OutOfBufferException();
            var buf = _buffer;
            buf[i] = (byte)value;
            buf[i + 1] = (byte)(value >> 8);
            buf[i + 2] = (byte)(value >> 16);
            buf[i + 3] = (byte)(value >> 24);
            buf[i + 4] = (byte)(value >> 32);
            buf[i + 5] = (byte)(value >> 40);
            buf[i + 6] = (byte)(value >> 48);
            buf[i + 7] = (byte)(value >> 56);
            _index = i + 8;
        }

        public void WriteVarInt32(uint value)
        {
            var i = _index;
            var len = _length;
            if (value < VarIntMore32 && i < len)
            {
                // one byte optimization
                _buffer[i] = (byte) value;
                _index = i + 1;
            }
            else
            {
                var buf = _buffer;
                // write "more" bytes
                while (value > VarIntData32 && i < len)
                {
                    buf[i++] = (byte) (value & VarIntData32 | VarIntMore32);
                    value >>= VarIntDataBits;
                }
                if (i == len)
                {
                    // can't write last byte - already at end of stream
                    // don't care how we got here
                    throw OutOfBufferException();
                }
                // write last byte
                buf[i++] = (byte) value;
                _index = i;
            }
        }

        public void WriteVarInt64(ulong value)
        {
            var i = _index;
            var len = _length;
            if (value < VarIntMore64 && i < len)
            {
                _buffer[i] = (byte)value;
                _index = i + 1;
            }
            else
            {
                var buf = _buffer;
                // write "more" bytes
                while (value > VarIntData64 && i < len)
                {
                    buf[i++] = (byte)(value & VarIntData64 | VarIntMore64);
                    value >>= VarIntDataBits;
                }
                if (i == len)
                {
                    // can't write last byte - already at end of stream
                    throw OutOfBufferException();
                }
                // write last byte
                buf[i++] = (byte)value;
                _index = i;
            }
        }
        
        public void WriteLittleEndianFloat(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                WriteBytes(bytes[0], bytes[1], bytes[2], bytes[3]);
            else
                WriteBytes(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        public void WriteDouble(double value)
        {
            WriteLittleEndian64((ulong)BitConverter.DoubleToInt64Bits(value));
        }

        private string _prepared;
        private int _preparedByteCount;
        public int PrepareUtf8String(string str)
        {
            _prepared = str;
            return _preparedByteCount = FastBufferInternal.Utf8Encoding.GetByteCount(str);
        }

        public static int GetUtf8StringSize(string str)
        {
            return str != null ? FastBufferInternal.Utf8Encoding.GetByteCount(str) : 0;
        }

        public void WriteRawUtf8String(string str)
        {
            if (string.IsNullOrEmpty(str))
                return;
            // reference equals here since Prepare should go before Write with the same string
            var byteCount = ReferenceEquals(str, _prepared) ? _preparedByteCount : FastBufferInternal.Utf8Encoding.GetByteCount(str);

            var index = _index;
            if (index + byteCount > _length)
                throw OutOfBufferException();

            if (byteCount == str.Length)
            {
                // Utf8 strings is ASCII, just write bytes
                var buf = _buffer;
                for (var i = 0; i < byteCount; ++i, ++index)
                    buf[index] = (byte) str[i];
                _index = index;
            }
            else
            {
                // Let encoding write it
                FastBufferInternal.Utf8Encoding.GetBytes(str, 0, str.Length, _buffer, index);
                _index = index + byteCount;
            }
        }

        public void WriteRawBytes(byte[] bytes)
        {
            var len = bytes.Length;
            if (len == 0)
                return;
            var i = _index;
            if (i + len > _length)
                throw OutOfBufferException();
            Array.Copy(bytes, 0, _buffer, i, len);
            _index = i + len;
        }

        private InvalidDataException OutOfBufferException()
        {
            // avoid further writing to ended stream
            _index = _length;
            return new InvalidDataException("Write attempt beyond end of buffer");
        }

        public BufferPosition CaptureCurrentPosition(int bytesBackwards = 0)
        {
            var i = _index;
            if (bytesBackwards < 0)
                throw EndOfWrittenExeption();
            if (i < _startIndex + bytesBackwards)
                throw StartOfStreamExeption();
            // minus one here since index is next written position and we have to capture last written
            return new BufferPosition(i - bytesBackwards);
        }

        private static InvalidOperationException StartOfStreamExeption()
        {
            return new InvalidOperationException("Position operation attempt before start of stream");
        }

        private static InvalidOperationException EndOfWrittenExeption()
        {
            return new InvalidOperationException("Position operation attempt after end of written data");
        }

        public byte Peek(BufferPosition position)
        {
            return _buffer[position.Index];
        }

        public byte Peek(BufferPosition position, int offset)
        {
            var index = position.Index + offset;
            if (index < _startIndex)
                throw StartOfStreamExeption();
            if (index >= _index)
                throw EndOfWrittenExeption();
            return _buffer[index];
        }

        public void WriteCopy(BufferPosition position, int count)
        {
            if (count <= 0)
                return;
            var i = _index;
            if (position.Index + count > _index)
                throw EndOfWrittenExeption();
            if (i + count > _length)
                throw OutOfBufferException();
            Array.Copy(_buffer, position.Index, _buffer, i, count);
            _index = i + count;
        }


        public struct BufferPosition
        {
            internal readonly int Index;

            internal BufferPosition(int index)
            {
                Index = index;
            }
        }
    }
}