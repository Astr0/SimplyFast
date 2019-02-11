using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SimplyFast.Serialization.Tests.Protobuf.TestData;

namespace SimplyFast.Serialization.Tests.Protobuf
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class WorksWithProtobufNet : MessageTests
    {
        protected override void Test(FTestMessage message, Action<FTestMessage> customAssert = null)
        {
            var d1 = FastWriteProtoReadOk(message);
            AssertDeserialized(message, d1, customAssert);
            var d2 = ProtoWriteFastReadOk(message);
            AssertDeserialized(message, d2, customAssert);
        }

        private static FTestMessage ProtoWriteFastReadOk(FTestMessage message)
        {
            using (var ms = new MemoryStream())
            {
                var pmsg = message.ToProtoNet();
                ProtoBuf.Serializer.Serialize(ms, pmsg);
                return pmsg.ToMessage();
            }
        }

        private static FTestMessage FastWriteProtoReadOk(FTestMessage message)
        {
            var serialized = ProtoSerializer.Serialize(message);
            using (var ms = new MemoryStream(serialized))
            {
                var deserialized = ProtoBuf.Serializer.Deserialize<PTestMessage>(ms);
                var fmsg = deserialized.ToMessage();
                return fmsg;
            }
        }
    }
}