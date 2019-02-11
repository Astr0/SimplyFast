using ProtoBuf;

namespace SimplyFast.Serialization.Tests.Protobuf.TestData
{
    [ProtoContract(Name = @"SomeEnum")]
    public enum SomeEnum
    {
        [ProtoEnum(Name = @"TEST_ENUM_VALUE0", Value = 0)]
        Value0 = 0,
        [ProtoEnum(Name = @"TEST_ENUM_VALUE1", Value = 1)]
        Value1 = 1
    }
}