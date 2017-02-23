using NUnit.Framework;

namespace SF.Serialization.Tests
{
    [TestFixture]
    public class WireFomatTests
    {
        private static void AssertTagOk(int field, WireType wireType)
        {
            var tag = WireFormat.MakeTag(field, wireType);
            var msg = $"Invalid: {field}, {wireType}";
            Assert.AreEqual(field, WireFormat.GetFieldNumber(tag), msg);
            Assert.AreEqual(wireType, WireFormat.GetWireType(tag), msg);
            Assert.AreEqual(wireType, (WireType) (tag & 7), msg);
            Assert.AreEqual(field, (int)(tag >> 3), msg);
        }

        private static void AssertAllFormats(int field)
        {
            AssertTagOk(field, WireType.StartGroup);
            AssertTagOk(field, WireType.EndGroup);
            AssertTagOk(field, WireType.Fixed32);
            AssertTagOk(field, WireType.Fixed64);
            AssertTagOk(field, WireType.LengthDelimited);
            AssertTagOk(field, WireType.Varint);
        }

        [Test]
        public void TestAll()
        {
            AssertAllFormats(1);
            AssertAllFormats(15);
            AssertAllFormats(16);
            AssertAllFormats(127);
            AssertAllFormats(1000);
            AssertAllFormats(10000);
        }
    }
}