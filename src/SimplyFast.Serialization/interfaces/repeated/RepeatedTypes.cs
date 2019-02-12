using System;
using System.Diagnostics.CodeAnalysis;
using T = SimplyFast.Serialization.RepeatedBaseType;

namespace SimplyFast.Serialization
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class RepeatedTypes
    {
        public static readonly RepeatedType<double> Double =
            Repeated(T.Double, x => x.ReadDouble(), (x, v) => x.WriteDouble(v));

        public static readonly RepeatedType<float> Float =
            Repeated(T.Float, x => x.ReadFloat(), (x, v) => x.WriteFloat(v));

        public static readonly RepeatedType<int> Int32 =
            Repeated(T.Int32, x => x.ReadInt32(), (x, v) => x.WriteInt32(v));

        public static readonly RepeatedType<long> Int64 =
            Repeated(T.Int64, x => x.ReadInt64(), (x, v) => x.WriteInt64(v));

        public static readonly RepeatedType<uint> UInt32 =
            Repeated(T.UInt32, x => x.ReadUInt32(), (x, v) => x.WriteUInt32(v));

        public static readonly RepeatedType<ulong> UInt64 =
            Repeated(T.UInt64, x => x.ReadUInt64(), (x, v) => x.WriteUInt64(v));

        public static readonly RepeatedType<int> SInt32 =
            Repeated(T.SInt32, x => x.ReadSInt32(), (x, v) => x.WriteSInt32(v));

        public static readonly RepeatedType<long> SInt64 =
            Repeated(T.SInt64, x => x.ReadSInt64(), (x, v) => x.WriteSInt64(v));

        public static readonly RepeatedType<uint> Fixed32 =
            Repeated(T.Fixed32, x => x.ReadFixed32(), (x, v) => x.WriteFixed32(v));

        public static readonly RepeatedType<ulong> Fixed64 =
            Repeated(T.Fixed64, x => x.ReadFixed64(), (x, v) => x.WriteFixed64(v));

        public static readonly RepeatedType<int> SFixed32 =
            Repeated(T.SFixed32, x => x.ReadSFixed32(), (x, v) => x.WriteSFixed32(v));

        public static readonly RepeatedType<long> SFixed64 =
            Repeated(T.SFixed64, x => x.ReadSFixed64(), (x, v) => x.WriteSFixed64(v));

        public static readonly RepeatedType<bool> Bool =
            Repeated(T.Bool, x => x.ReadBool(), (x, v) => x.WriteBool(v));

        public static readonly RepeatedType<string> String =
            Repeated(T.String, x => x.ReadString(), (x, v) => x.WriteString(v));

        public static readonly RepeatedType<byte[]> Bytes =
            Repeated(T.Bytes, x => x.ReadBytes(), (x, v) => x.WriteBytes(v));

        public static readonly RepeatedType<byte[]> SerializedMessage =
            Repeated(T.Message, x => x.ReadBytes(), (x, v) => x.WriteBytes(v));

        private static RepeatedType<TField> Repeated<TField>(T baseType, Func<IInputStream, TField> read,
            Action<IOutputStream, TField> write)
        {
            return new RepeatedType<TField>(baseType, read, write);
        }
        //public static readonly RepeatedType<int> Enum =
        //    Repeated(T.Enum, x => x.ReadEnum(), (x, v) => x.WriteEnum(v));

        public static RepeatedType<TEnum> Enum<TEnum>(Func<int, TEnum> toEnum, Func<TEnum, int> toInt)
            where TEnum : struct
        {
            return Repeated(T.Enum, x => toEnum(x.ReadEnum()), (x, v) => x.WriteEnum(toInt(v)));
        }

        public static RepeatedType<TMessage> Message<TMessage>(Func<TMessage> create) where TMessage : IMessage
        {
            return Repeated(T.Message, x =>
            {
                var m = create();
                x.ReadMessage(m);
                return m;
            }, (x, v) => x.WriteMessage(v));
        }

        public static RepeatedType<TMessage> Message<TMessage>() where TMessage : IMessage, new()
        {
            return Repeated(T.Message, x =>
            {
                var m = new TMessage();
                x.ReadMessage(m);
                return m;
            }, (x, v) => x.WriteMessage(v));
        }
    }
}