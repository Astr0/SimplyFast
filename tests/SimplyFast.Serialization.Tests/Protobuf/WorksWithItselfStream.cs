using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SimplyFast.Serialization.Tests.Protobuf.TestData;

namespace SimplyFast.Serialization.Tests.Protobuf
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class WorksWithItselfStream : MessageTests
    {
        protected override void Test(FTestMessage message, Action<FTestMessage> customAssert = null)
        {
            using (var ms = new MemoryStream())
            {
                ProtoSerializer.Serialize(ms, message);
                ms.Position = 0;
                var deserialized = ProtoSerializer.Deserialize<FTestMessage>(ms);
                AssertDeserialized(message, deserialized, customAssert);
            }
        }
    }
}