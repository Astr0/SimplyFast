using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.IO;
using SimplyFast.Serialization.interfaces;
using SimplyFast.Serialization.interfaces.repeated;

namespace SimplyFast.Serialization.Protobuf
{
    /// <summary>
    /// Root-cached implementation used internally. Doesn't allow GetMessageSize for other messages 
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal class ProtoSizeCalc : IOutputStream
    {
        private int _lastTagSize;
        private SizeCache _messageSizes;
        private int _size;
        private byte _tagFirstByte;

        public ProtoSizeCalc(IMessage root)
        {
            // use cache aways since it speeds up size calculation for embedded messages
            // A has B and C, B has D, D has E
            // with cache it's one pass
            // without it's A, B, C, B, D, E
            // C, B, D, E
            // B, D, E
            // D, E
            // E

            root.WriteTo(this);
            Size = _size;
        }

        private SizeCache MessageSizes => _messageSizes ?? (_messageSizes = new SizeCache());

        public int Size { get; }

        void IOutputStream.WriteRawTag(byte b1)
        {
            _size++;
            _lastTagSize = 1;
            _tagFirstByte = b1;
        }

        void IOutputStream.WriteRawTag(byte b1, byte b2)
        {
            _size += 2;
            _lastTagSize = 2;
            _tagFirstByte = b1;
        }

        void IOutputStream.WriteRawTag(byte b1, byte b2, byte b3)
        {
            _size += 3;
            _lastTagSize = 3;
            _tagFirstByte = b1;
        }

        void IOutputStream.WriteRawTag(byte b1, byte b2, byte b3, byte b4)
        {
            _size += 4;
            _lastTagSize = 4;
            _tagFirstByte = b1;
        }

        void IOutputStream.WriteRawTag(byte b1, byte b2, byte b3, byte b4, byte b5)
        {
            _size += 5;
            _lastTagSize = 5;
            _tagFirstByte = b1;
        }

        void IOutputStream.WriteDouble(double value)
        {
            _size += 8;
        }

        void IOutputStream.WriteFloat(float value)
        {
            _size += 4;
        }

        public void WriteInt32(int value)
        {
            _size += value >= 0 ? VarIntHelper.GetVarInt32Size((uint) value) : 10;
        }

        public void WriteInt64(long value)
        {
            _size += VarIntHelper.GetVarInt64Size((ulong) value);
        }

        public void WriteUInt32(uint value)
        {
            _size += VarIntHelper.GetVarInt32Size(value);
        }

        public void WriteUInt64(ulong value)
        {
            _size += VarIntHelper.GetVarInt64Size(value);
        }

        public void WriteSInt32(int value)
        {
            _size += VarIntHelper.GetVarInt32Size(ProtoOutputStream.EncodeZigZag32(value));
        }

        public void WriteSInt64(long value)
        {
            _size += VarIntHelper.GetVarInt64Size(ProtoOutputStream.EncodeZigZag64(value));
        }

        void IOutputStream.WriteFixed32(uint value)
        {
            _size += 4;
        }

        void IOutputStream.WriteFixed64(ulong value)
        {
            _size += 8;
        }

        void IOutputStream.WriteSFixed32(int value)
        {
            _size += 4;
        }

        void IOutputStream.WriteSFixed64(long value)
        {
            _size += 8;
        }

        void IOutputStream.WriteBool(bool value)
        {
            _size++;
        }

        void IOutputStream.WriteString(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var size = FastBufferWriter.GetUtf8StringSize(value);
                _size += VarIntHelper.GetVarInt32Size((uint) size) + size;
            }
            else
            {
                _size++;
            }
        }

        void IOutputStream.WriteBytes(byte[] value)
        {
            if (value != null && value.Length != 0)
            {
                var size = value.Length;
                _size += VarIntHelper.GetVarInt32Size((uint) size) + size;
            }
            else
            {
                _size++;
            }
        }

        void IOutputStream.WriteEnum(int value)
        {
            _size += value >= 0 ? VarIntHelper.GetVarInt32Size((uint) value) : 10;
        }

        void IOutputStream.WriteMessage(IMessage message)
        {
            // we need to get individual message size here
            // to get message length varint size...
            var current = _size;

            var spot = MessageSizes.Reserve();
            // just write it to this "stream"
            message.WriteTo(this);
            // ok, we have size, order doesn't matter ^^
            var size = _size - current;
            // we can event cache it somewhere
            _messageSizes.Set(spot, size);

            // oh, and add length prefix to the "stream"
            _size += VarIntHelper.GetVarInt32Size((uint) size);
        }

        public void WriteRepeated<T>(IReadOnlyCollection<T> values, RepeatedType<T> type)
        {
            // here we can check by one byte since wire type is mask and written in first byte
            // acually we can quick check by BaseType and don't store first byte, but 
            // to support some strange non-packed primitive types...
            if (ProtoRepeatedInfo<T>.IsPackedRepeatedField(_tagFirstByte))
            {
                // packed 
                // fixed type?
                var fixedBaseSize = GetFixedBaseTypeSize(type.BaseType);
                if (fixedBaseSize == 0)
                {
                    // variable field..., use same hack as in WriteMessage
                    var current = _size;
                    var write = type.WriteElement;

                    var spot = MessageSizes.Reserve();
                    foreach (var value in values)
                        write(this, value);
                    var size = _size - current;
                    // remember message size, we will need it
                    _messageSizes.Set(spot, size);

                    _size += VarIntHelper.GetVarInt32Size((uint) size);
                }
                else
                {
                    // we can calculate message size fast and easy, no need to cache
                    var size = fixedBaseSize * values.Count;
                    _size += VarIntHelper.GetVarInt32Size((uint) size) + size;
                }
            }
            else
            {
                // don't cache non-packed sizes, they won't be asked
                // all the tag sizes
                _size += _lastTagSize * (values.Count - 1);
                var fixedBaseSize = GetFixedBaseTypeSize(type.BaseType);
                if (fixedBaseSize == 0)
                {
                    var write = type.WriteElement;
                    // write last tag
                    foreach (var value in values)
                        write(this, value);
                }
                else
                {
                    _size += fixedBaseSize * values.Count;
                }
            }
        }

        public int GetRepeatedSize<T>(IReadOnlyCollection<T> values, RepeatedType<T> type)
        {
            if (values == null || values.Count == 0)
                return 0;
            var fixedBaseSize = GetFixedBaseTypeSize(type.BaseType);
            if (fixedBaseSize != 0)
                return fixedBaseSize * values.Count;

            //if (_cache)
            //{
            if (_messageSizes == null)
                throw NoCalcMessagesException();
            // we can cachy by collection only since Repeated type has no out-of-lib configuration
            var size = _messageSizes.Get();
            if (size < 0)
                throw new InvalidOperationException("Root message doesn't had passed repeated field");
            return size;
            //}
            //// no cache, calculate always
            //var captureSize = _size;
            //_size = 0;
            //WriteRepeated(values, type);
            //var result = _size;
            //_size = captureSize;
            //return result;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public int GetMessageSize(IMessage message)
        {
            //if (_cache)
            //{
            if (_messageSizes == null)
                throw NoCalcMessagesException();
            var size = _messageSizes.Get();
            if (size < 0)
                throw new InvalidOperationException("Root message doesn't had passed message");
            return size;
            //}
            //// no cache, calculate always
            //var captureSize = _size;
            //_size = 0;
            //message.WriteTo(this);
            //var result = _size;
            //_size = captureSize;
            //return result;
        }

        private static InvalidOperationException NoCalcMessagesException()
        {
            return new InvalidOperationException("Root message graph had no fields with variable size");
        }

        internal static int GetFixedBaseTypeSize(RepeatedBaseType type)
        {
            switch (type)
            {
                case RepeatedBaseType.Bool:
                    return 1;
                case RepeatedBaseType.Fixed32:
                case RepeatedBaseType.SFixed32:
                case RepeatedBaseType.Float:
                    return 4;
                case RepeatedBaseType.Fixed64:
                case RepeatedBaseType.SFixed64:
                case RepeatedBaseType.Double:
                    return 8;
                default:
                    return 0;
            }
        }

        private class SizeCache
        {
            //private readonly List<KeyValuePair<object, int>> _cache = new KeyValuePairList<object, int>();
            //private static readonly IEqualityComparer<object> _comparer = ReferenceEqualityComparer.Default;
            private readonly List<int> _cache = new List<int>();
            private int _readIndex;

            public ReservedSpot Reserve()
            {
                //_cache.Add(new KeyValuePair<object, int>());
                _cache.Add(-1);
                return new ReservedSpot(_cache.Count - 1);
            }

            public void Set(ReservedSpot spot, int size)
            {
                _cache[spot.Index] = size;
            }

            public int Get()
            {
                return _cache[_readIndex++];
            }


            //public void Set(ReservedSpot spot, object obj, int size)
            //{
            //    _cache[spot.Index] = new KeyValuePair<object, int>(obj, size);
            //}

            //public int Get(object obj)
            //{
            //    var kvp = _cache[_readIndex++];
            //    if (!_comparer.Equals(kvp.Key, obj))
            //        throw new InvalidOperationException("Next cached should be next read");
            //    return kvp.Value;
            //}

            public struct ReservedSpot
            {
                public readonly int Index;

                public ReservedSpot(int index)
                {
                    Index = index;
                }
            }
        }
    }
}