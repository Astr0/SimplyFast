using System;
using System.IO;
using SimplyFast.IO;
using SimplyFast.Serialization.interfaces;
using SimplyFast.Serialization.interfaces.repeated;

namespace SimplyFast.Serialization.Protobuf
{
    internal class ProtoInputStream : FastBufferReader, IInputStream
    {
        private static readonly byte[] EmptyBytes = new byte[0];
        private uint _lastTag;
        private uint _nextTag;

        private bool _nextTagRead;

        public ProtoInputStream(byte[] buffer, int index, int count)
            : base(buffer, index, count)
        {
        }

        public ProtoInputStream(byte[] buffer)
            : base(buffer)
        {
        }

        public void SkipField()
        {
            if (_lastTag == 0U)
            {
                if (End)
                    throw new InvalidOperationException("SkipField cannot be called at the end of a stream");
                throw new InvalidOperationException("SkipField cannot be called when tag have not been read");
            }
            switch (WireFormat.GetWireType(_lastTag))
            {
                case WireType.Varint:
                    SkipVarInt();
                    break;
                case WireType.Fixed32:
                    SkipBytes(4);
                    break;
                case WireType.Fixed64:
                    SkipBytes(8);
                    break;
                case WireType.LengthDelimited:
                    SkipBytes(ReadByteSize());
                    break;
                case WireType.StartGroup:
                    SkipGroup();
                    break;
                case WireType.EndGroup:
                    throw new InvalidDataException(
                        "SkipField called on an end-group tag. Most likely start group was missing.");
            }
        }

        public uint ReadTag()
        {
            if (_nextTagRead)
            {
                _lastTag = _nextTag;
                _nextTagRead = false;
                return _lastTag;
            }
            if (End)
            {
                _lastTag = 0U;
                return 0U;
            }
            _lastTag = ReadVarInt32();
            if (_lastTag == 0U)
                throw InvalidTagException();
            return _lastTag;
        }

        public float ReadFloat()
        {
            return ReadLittleEndianFloat();
        }

        public int ReadInt32()
        {
            return (int) ReadVarInt32();
        }

        public long ReadInt64()
        {
            return (long) ReadVarInt64();
        }

        public uint ReadUInt32()
        {
            return ReadVarInt32();
        }

        public ulong ReadUInt64()
        {
            return ReadVarInt64();
        }

        public int ReadSInt32()
        {
            return DecodeZigZag32(ReadVarInt32());
        }

        public long ReadSInt64()
        {
            return DecodeZigZag64(ReadVarInt64());
        }

        public uint ReadFixed32()
        {
            return ReadLittleEndian32();
        }

        public ulong ReadFixed64()
        {
            return ReadLittleEndian64();
        }

        public int ReadSFixed32()
        {
            return (int) ReadLittleEndian32();
        }

        public long ReadSFixed64()
        {
            return (long) ReadLittleEndian64();
        }

        public bool ReadBool()
        {
            return ReadVarInt32() > 0U;
        }

        public string ReadString()
        {
            // length, then byte string
            var length = ReadByteSize();
            return length != 0 ? ReadRawUtf8String(length) : "";
        }

        public byte[] ReadBytes()
        {
            // length, then bytes
            var length = ReadByteSize();
            return length != 0 ? ReadRawBytes(length) : EmptyBytes;
        }

        public int ReadEnum()
        {
            // enums are coded as variants in protobuf, right?
            return (int) ReadVarInt32();
        }

        public void ReadMessage(IMessage message)
        {
            // read message length
            var size = ReadByteSize();
            // set length limit for this message
            var captureLength = SetView(size);
            message.ReadFrom(this);
            RestoreView(captureLength);
        }

        public void ReadRepeated<T>(AddRepeatedItem<T> addItem, RepeatedType<T> type,
            PrepareForRepeatedItems prepareForRepeatedItems = null)
        {
            var read = type.ReadElement;

            var tag = _lastTag;
            if (ProtoRepeatedInfo<T>.IsPackedRepeatedField(tag))
            {
                // packed

                // read repeated size
                var totalSize = ReadByteSize();
                if (totalSize <= 0)
                    return;

                // try call prepare to add items
                if (prepareForRepeatedItems != null)
                {
                    var fixedSize = ProtoSizeCalc.GetFixedBaseTypeSize(type.BaseType);
                    if (fixedSize != 0)
                    {
                        var count = totalSize / fixedSize;
                        prepareForRepeatedItems(count);
                    }
                }

                // set length limit for this message
                var captureLength = SetView(totalSize);
                // end here will return view's end
                while (!End)
                    addItem(read(this));
                RestoreView(captureLength);
            }
            else
            {
                do
                {
                    addItem(read(this));
                } while (ReadTagIfEquals(tag));
            }
        }

        private void SkipGroup()
        {
            var startGroupTag = _lastTag;
            while (true)
            {
                var tag = ReadTag();
                if (tag == 0U)
                    throw OutOfInputException();
                if (WireFormat.GetWireType(tag) != WireType.EndGroup)
                {
                    SkipField();
                    continue;
                }
                var startGroupFieldNumber = WireFormat.GetFieldNumber(startGroupTag);
                var endGroupFieldNumber = WireFormat.GetFieldNumber(tag);
                if (startGroupFieldNumber == endGroupFieldNumber)
                    return;

                var message =
                    $"Mismatched end-group tag. Started with field {startGroupFieldNumber}; ended with field {endGroupFieldNumber}";
                throw new InvalidDataException(message);
            }
        }

        private uint PeekTag()
        {
            if (_nextTagRead)
                return _nextTag;
            // this will be overwritten by ReadTag
            var lastTag = _lastTag;
            _nextTag = ReadTag();
            _nextTagRead = true;
            _lastTag = lastTag;
            return _nextTag;
        }

        private static Exception InvalidTagException()
        {
            return new InvalidDataException("Invalid Tag = 0");
        }

        private int ReadByteSize()
        {
            var len = (int) ReadVarInt32();
            if (len < 0)
                throw new InvalidDataException("Length is less than zero");
            return len;
        }

        private static int DecodeZigZag32(uint n)
        {
            return (int) (n >> 1) ^ -((int) n & 1);
        }

        private static long DecodeZigZag64(ulong n)
        {
            return (long) (n >> 1) ^ -((long) n & 1L);
        }

        private bool ReadTagIfEquals(uint tag)
        {
            if (PeekTag() != tag)
                return false;
            _nextTagRead = false;
            return true;
        }
    }
}