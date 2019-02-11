using Xunit;
using SimplyFast.Serialization.interfaces;

namespace SimplyFast.Serialization.Tests
{
    
    public class WireFomatTests
    {
        private static void AssertTagOk(int field, WireType wireType)
        {
            var tag = WireFormat.MakeTag(field, wireType);
            Assert.Equal(field, WireFormat.GetFieldNumber(tag));
            Assert.Equal(wireType, WireFormat.GetWireType(tag));
            Assert.Equal(wireType, (WireType) (tag & 7));
            Assert.Equal(field, (int)(tag >> 3));
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

        [Fact]
        public void AllFine()
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