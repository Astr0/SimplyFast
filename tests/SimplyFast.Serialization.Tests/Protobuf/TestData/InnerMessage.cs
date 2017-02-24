using System.Diagnostics;
using ProtoBuf;
using SimplyFast.Serialization.interfaces;

namespace SimplyFast.Serialization.Tests.Protobuf.TestData
{
    [ProtoContract(Name = @"InnerMessage", UseProtoMembersOnly = true)]
    public partial class InnerMessage : IMessage
    {
        // Fields
        private int _test;

        // Properties
        [ProtoMember(1, IsRequired = true, Name = @"test", DataFormat = DataFormat.TwosComplement)]
        public int Test
        {
            get { return _test; }
            set { _test = value; }
        }

        [DebuggerNonUserCode]
        void IMessage.WriteTo(IOutputStream output)
        {
            output.WriteRawTag(8);
            output.WriteInt32(_test);
        }

        [DebuggerNonUserCode]
        void IMessage.ReadFrom(IInputStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
                switch (tag)
                {
                    case 8:
                        _test = input.ReadInt32();
                        break;
                    default:
                        input.SkipField();
                        break;
                }
        }
    }
}