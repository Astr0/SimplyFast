using System;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.Serialization.Tests.Protobuf.TestData;

namespace SimplyFast.Serialization.Tests.Protobuf
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class WorksWithItselfPooled : MessageTests
    {
        protected override void Test(FTestMessage message, Action<FTestMessage> customAssert = null)
        {
            using (var pool = ProtoSerializer.SerializePooled(message))
            {
                var buf = pool.Instance;
                var deserialized = ProtoSerializer.Deserialize<FTestMessage>(buf.Buffer, buf.Offset, buf.Count);
                AssertDeserialized(message, deserialized, customAssert);
            }
        }
    }
}