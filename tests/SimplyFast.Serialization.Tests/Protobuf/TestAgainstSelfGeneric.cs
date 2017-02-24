using System;
using System.IO;
using NUnit.Framework;
using SimplyFast.Serialization.Tests.Protobuf.TestData;

namespace SimplyFast.Serialization.Tests.Protobuf
{
    [TestFixture]
    public class TestAgainstSelfGeneric : MessageTests
    {
        protected override void Test(FTestMessage message, Action<FTestMessage> customAssert = null)
        {
            using (var ms = new MemoryStream())
            {
                ProtoSerializer.Serialize(ms, message);
                ms.Position = 0;
                var deserialized = (FTestMessage) ProtoSerializer.Deserialize(typeof(FTestMessage), ms);
                AssertDeserialized(message, deserialized, customAssert);
            }
        }
    }
}