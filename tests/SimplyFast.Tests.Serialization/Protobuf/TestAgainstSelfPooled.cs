using System;
using NUnit.Framework;
using SF.Serialization;
using SF.Tests.Serialization.Protobuf.TestData;

namespace SF.Tests.Serialization.Protobuf
{
    [TestFixture]
    public class TestAgainstSelfPooled : MessageTests
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