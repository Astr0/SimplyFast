using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SF.Serialization
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public interface IOutputStream
    {
        // Tag should be written at one call so fancy serializers like Json can write some string
        void WriteRawTag(byte b1);
        void WriteRawTag(byte b1, byte b2);
        void WriteRawTag(byte b1, byte b2, byte b3);
        void WriteRawTag(byte b1, byte b2, byte b3, byte b4);
        void WriteRawTag(byte b1, byte b2, byte b3, byte b4, byte b5);

        void WriteDouble(double value);
        void WriteFloat(float value);
        void WriteInt32(int value);
        void WriteInt64(long value);
        void WriteUInt32(uint value);
        void WriteUInt64(ulong value);
        void WriteSInt32(int value);
        void WriteSInt64(long value);
        void WriteFixed32(uint value);
        void WriteFixed64(ulong value);
        void WriteSFixed32(int value);
        void WriteSFixed64(long value);
        void WriteBool(bool value);
        void WriteString(string value);
        void WriteBytes(byte[] value);

        // special case for some fancy streams like Json, Bson, etc.
        void WriteEnum(int value);

        // Write message 
        void WriteMessage(IMessage message);

        // TODO: Repeated
        // void WriteLength(int length);
        void WriteRepeated<T>(IReadOnlyCollection<T> values, RepeatedType<T> type);
    }
}