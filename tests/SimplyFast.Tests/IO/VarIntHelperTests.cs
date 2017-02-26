using System.Linq;
using Xunit;
using SimplyFast.IO;

namespace SimplyFast.Tests.IO
{
    
    public class VarIntHelperTests
    {
        [Fact]
        public void GetVarInt32SizeWorks()
        {
            Assert.Equal(1, VarIntHelper.GetVarInt32Size(0));
            for (var i = 1; i < 5; i++)
            {
                Assert.Equal(i, VarIntHelper.GetVarInt32Size((1U << (7 * i)) - 1));
                Assert.Equal(i + 1, VarIntHelper.GetVarInt32Size(1U << (7 * i)));
            }
        }

        [Fact]
        public void GetVarInt64SizeWorks()
        {
            Assert.Equal(1, VarIntHelper.GetVarInt64Size(0));
            for (var i = 1; i < 10; i++)
            {
                Assert.Equal(i, VarIntHelper.GetVarInt64Size((1UL << (7 * i)) - 1));
                Assert.Equal(i + 1, VarIntHelper.GetVarInt64Size(1UL << (7 * i)));
            }
        }

        [Fact]
        public void GetVarInt32BytesSeemsOk()
        {
            var b0 = VarIntHelper.GetVarInt32Bytes(0U);
            Assert.Equal(1, b0.Length);
            Assert.Equal(0, b0[0]);
            for (var i = 1; i < 5; i++)
            {
                var breakingPoint = VarIntHelper.GetVarInt32Bytes((1U << (7 * i)) - 1);
                Assert.Equal(i, breakingPoint.Length);
                Assert.True(breakingPoint.Take(i - 1).All(x => x == 255));
                Assert.Equal(127, breakingPoint.Last());
                var next = VarIntHelper.GetVarInt32Bytes(1U << (7 * i));
                Assert.Equal(i + 1, next.Length);
                Assert.True(next.Take(i).All(x => x == 128));
                Assert.Equal(1, next.Last());
            }
        }

        [Fact]
        public void GetVarInt64BytesSeemsOk()
        {
            var b0 = VarIntHelper.GetVarInt64Bytes(0U);
            Assert.Equal(1, b0.Length);
            Assert.Equal(0, b0[0]);
            for (var i = 1; i < 10; i++)
            {
                var breakingPoint = VarIntHelper.GetVarInt64Bytes((1UL << (7 * i)) - 1);
                Assert.Equal(i, breakingPoint.Length);
                Assert.True(breakingPoint.Take(i - 1).All(x => x == 255));
                Assert.Equal(127, breakingPoint.Last());
                var next = VarIntHelper.GetVarInt64Bytes(1UL << (7 * i));
                Assert.Equal(i + 1, next.Length);
                Assert.True(next.Take(i).All(x => x == 128));
                Assert.Equal(1, next.Last());
            }
        }
    }
}