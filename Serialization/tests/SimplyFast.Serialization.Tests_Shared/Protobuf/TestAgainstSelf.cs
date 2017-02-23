using System;
using NUnit.Framework;
using SF.Serialization.Tests.Protobuf.TestData;

namespace SF.Serialization.Tests.Protobuf
{
    [TestFixture]
    public class TestAgainstSelf : MessageTests
    {
        protected override void Test(FTestMessage message, Action<FTestMessage> customAssert = null)
        {
            var serialized = ProtoSerializer.Serialize(message);
            var deserialized = ProtoSerializer.Deserialize<FTestMessage>(serialized);
            AssertDeserialized(message, deserialized, customAssert);
        }
    }
}