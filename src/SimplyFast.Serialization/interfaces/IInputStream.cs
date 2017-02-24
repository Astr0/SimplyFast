using System.Diagnostics.CodeAnalysis;
using SimplyFast.Serialization.interfaces.repeated;

namespace SimplyFast.Serialization.interfaces
{
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IInputStream
    {
        /// <summary>
        /// Skip unknown field
        /// </summary>
        void SkipField();

        /// <summary>
        /// returns raw tag or 0 at end of stream
        /// </summary>
        //uint PeekTag();
        uint ReadTag();

        double ReadDouble();
        float ReadFloat();
        int ReadInt32();
        long ReadInt64();
        uint ReadUInt32();
        ulong ReadUInt64();
        int ReadSInt32();
        long ReadSInt64();
        uint ReadFixed32();
        ulong ReadFixed64();
        int ReadSFixed32();
        long ReadSFixed64();
        bool ReadBool();
        string ReadString();
        byte[] ReadBytes();

        // special case for some fancy streams like Json, Bson, etc.
        int ReadEnum();

        // read message 
        void ReadMessage(IMessage message);

        // this thing should have some add method and ensure capacity for fixed size stuff
        void ReadRepeated<T>(AddRepeatedItem<T> addItem, RepeatedType<T> type, PrepareForRepeatedItems prepareForRepeatedItems = null);
    }
}