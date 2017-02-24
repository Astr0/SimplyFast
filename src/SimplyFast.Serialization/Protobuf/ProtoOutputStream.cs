using System.Collections.Generic;
using SimplyFast.IO;
using SimplyFast.Serialization.interfaces;
using SimplyFast.Serialization.interfaces.repeated;

namespace SimplyFast.Serialization.Protobuf
{
    internal class ProtoOutputStream : FastBufferWriter, IOutputStream
    {
        private readonly ProtoSizeCalc _sizeCalc;

        private int _lastTagBytes;

        public ProtoOutputStream(ProtoSizeCalc sizeCalc, byte[] buffer, int index, int count)
            : base(buffer, index, count)
        {
            _sizeCalc = sizeCalc;
        }

        public ProtoOutputStream(ProtoSizeCalc sizeCalc, byte[] buffer) : base(buffer)
        {
            _sizeCalc = sizeCalc;
        }

        public void WriteRawTag(byte b1)
        {
            WriteByte(b1);
            _lastTagBytes = 1;
        }

        public void WriteRawTag(byte b1, byte b2)
        {
            WriteBytes(b1, b2);
            _lastTagBytes = 2;
        }

        public void WriteRawTag(byte b1, byte b2, byte b3)
        {
            WriteBytes(b1, b2, b3);
            _lastTagBytes = 3;
        }

        public void WriteRawTag(byte b1, byte b2, byte b3, byte b4)
        {
            WriteBytes(b1, b2, b3, b4);
            _lastTagBytes = 4;
        }

        public void WriteRawTag(byte b1, byte b2, byte b3, byte b4, byte b5)
        {
            WriteBytes(b1, b2, b3, b4, b5);
            _lastTagBytes = 5;
        }

        public void WriteFloat(float value)
        {
            WriteLittleEndianFloat(value);
        }

        public void WriteInt32(int value)
        {
            if (value >= 0)
                WriteVarInt32((uint) value);
            else
                WriteVarInt64((ulong) value);
        }

        public void WriteInt64(long value)
        {
            WriteVarInt64((ulong) value);
        }

        public void WriteUInt32(uint value)
        {
            WriteVarInt32(value);
        }

        public void WriteUInt64(ulong value)
        {
            WriteVarInt64(value);
        }

        public void WriteSInt32(int value)
        {
            WriteVarInt32(EncodeZigZag32(value));
        }

        public void WriteSInt64(long value)
        {
            WriteVarInt64(EncodeZigZag64(value));
        }

        public void WriteFixed32(uint value)
        {
            WriteLittleEndian32(value);
        }

        public void WriteFixed64(ulong value)
        {
            WriteLittleEndian64(value);
        }

        public void WriteSFixed32(int value)
        {
            WriteLittleEndian32((uint) value);
        }

        public void WriteSFixed64(long value)
        {
            WriteLittleEndian64((ulong) value);
        }

        public void WriteBool(bool value)
        {
            WriteByte(value ? (byte) 1 : (byte) 0);
        }

        public void WriteString(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var byteCount = PrepareUtf8String(value);
                WriteByteSize(byteCount);
                WriteRawUtf8String(value);
            }
            else
            {
                WriteByte(0);
            }
        }

        public void WriteBytes(byte[] value)
        {
            if (value != null && value.Length != 0)
            {
                WriteByteSize(value.Length);
                WriteRawBytes(value);
            }
            else
            {
                WriteByte(0);
            }
        }

        public void WriteEnum(int value)
        {
            // would be great to use VarInt32 here, but enums can be negative...
            if (value >= 0)
                WriteVarInt32((uint) value);
            else
                WriteVarInt64((ulong) value);
        }

        public void WriteMessage(IMessage message)
        {
            WriteByteSize(_sizeCalc.GetMessageSize(message));
            // trust message, don't set length-limit view
            message.WriteTo(this);
        }

        public void WriteRepeated<T>(IReadOnlyCollection<T> values, RepeatedType<T> type)
        {
            var tagBytes = _lastTagBytes;
            var tagStartPosition = CaptureCurrentPosition(tagBytes);
            // here we can check by one byte since wire type is mask and written in first byte
            if (ProtoRepeatedInfo<T>.IsPackedRepeatedField(Peek(tagStartPosition)))
            {
                // packed 
                var size = _sizeCalc.GetRepeatedSize(values, type);
                WriteByteSize(size);
                if (size == 0)
                    return;
                // trust message, don't set length-limit view
                var write = type.WriteElement;
                foreach (var value in values)
                    write(this, value);
            }
            else
            {
                var write = type.WriteElement;
                var left = values.Count;
                foreach (var value in values)
                {
                    write(this, value);
                    left--;
                    if (left == 0)
                        break;
                    // write last tag
                    WriteCopy(tagStartPosition, tagBytes);
                }
            }
        }

        internal static uint EncodeZigZag32(int n)
        {
            return (uint) ((n << 1) ^ (n >> 31));
        }

        internal static ulong EncodeZigZag64(long n)
        {
            return (ulong) ((n << 1) ^ (n >> 63));
        }

        private void WriteByteSize(int size)
        {
            WriteVarInt32((uint) size);
        }
    }
}