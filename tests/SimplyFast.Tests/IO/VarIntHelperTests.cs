using System.Linq;
using NUnit.Framework;
using SimplyFast.IO;

namespace SimplyFast.Tests.IO
{
    [TestFixture]
    public class VarIntHelperTests
    {
        [Test]
        public void GetVarInt32SizeWorks()
        {
            Assert.AreEqual(1, VarIntHelper.GetVarInt32Size(0));
            for (var i = 1; i < 5; i++)
            {
                Assert.AreEqual(i, VarIntHelper.GetVarInt32Size((1U << (7 * i)) - 1));
                Assert.AreEqual(i + 1, VarIntHelper.GetVarInt32Size(1U << (7 * i)));
            }
        }

        [Test]
        public void GetVarInt64SizeWorks()
        {
            Assert.AreEqual(1, VarIntHelper.GetVarInt64Size(0));
            for (var i = 1; i < 10; i++)
            {
                Assert.AreEqual(i, VarIntHelper.GetVarInt64Size((1UL << (7 * i)) - 1));
                Assert.AreEqual(i + 1, VarIntHelper.GetVarInt64Size(1UL << (7 * i)));
            }
        }

        [Test]
        public void GetVarInt32BytesSeemsOk()
        {
            var b0 = VarIntHelper.GetVarInt32Bytes(0U);
            Assert.AreEqual(1, b0.Length);
            Assert.AreEqual(0, b0[0]);
            for (var i = 1; i < 5; i++)
            {
                var breakingPoint = VarIntHelper.GetVarInt32Bytes((1U << (7 * i)) - 1);
                Assert.AreEqual(i, breakingPoint.Length);
                Assert.IsTrue(breakingPoint.Take(i - 1).All(x => x == 255));
                Assert.AreEqual(127, breakingPoint.Last());
                var next = VarIntHelper.GetVarInt32Bytes(1U << (7 * i));
                Assert.AreEqual(i + 1, next.Length);
                Assert.IsTrue(next.Take(i).All(x => x == 128));
                Assert.AreEqual(1, next.Last());
            }
        }

        [Test]
        public void GetVarInt64BytesSeemsOk()
        {
            var b0 = VarIntHelper.GetVarInt64Bytes(0U);
            Assert.AreEqual(1, b0.Length);
            Assert.AreEqual(0, b0[0]);
            for (var i = 1; i < 10; i++)
            {
                var breakingPoint = VarIntHelper.GetVarInt64Bytes((1UL << (7 * i)) - 1);
                Assert.AreEqual(i, breakingPoint.Length);
                Assert.IsTrue(breakingPoint.Take(i - 1).All(x => x == 255));
                Assert.AreEqual(127, breakingPoint.Last());
                var next = VarIntHelper.GetVarInt64Bytes(1UL << (7 * i));
                Assert.AreEqual(i + 1, next.Length);
                Assert.IsTrue(next.Take(i).All(x => x == 128));
                Assert.AreEqual(1, next.Last());
            }
        }
    }
}