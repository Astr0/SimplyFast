using System;
using System.Collections.Generic;
using NUnit.Framework;
using SF.Serialization.Tests.Protobuf.TestData;

namespace SF.Serialization.Tests.Protobuf
{
    public abstract class MessageTests
    {
        protected abstract void Test(FTestMessage message, Action<FTestMessage> customAssert = null);

        protected void AssertDeserialized(FTestMessage message, FTestMessage deserialized, Action<FTestMessage> customAssert)
        {
            if (customAssert != null)
                customAssert(deserialized);
            else
                Assert.AreEqual(message, deserialized);
        }

        [Test]
        public void EmptyOk()
        {
            Test(new FTestMessage());
        }

        [Test]
        public void FloatOk()
        {
            Array.ForEach(
                new[]
                {
                    123, 23423f, float.NaN, float.MaxValue, float.MinValue, float.Epsilon, float.NegativeInfinity,
                    float.PositiveInfinity
                },
                f =>
                    Test(new FTestMessage
                    {
                        Ffloat = f
                    }));
        }

        [Test]
        public void Int32Ok()
        {
            Array.ForEach(
                new []
                {
                    123, 0, -234, -int.MaxValue, 23423, int.MaxValue
                },
                f =>
                    Test(new FTestMessage
                    {
                        Fint32 = f
                    }));
        }

        [Test]
        public void Int64Ok()
        {
            Array.ForEach(
                new []
                {
                    123, 0, -234, -int.MaxValue, 23423, int.MaxValue, long.MinValue, long.MaxValue
                },
                f =>
                    Test(new FTestMessage
                    {
                        Fint64 = f
                    }));
        }

        [Test]
        public void UInt32Ok()
        {
            Array.ForEach(
                new uint[]
                {
                    123, 0, uint.MaxValue, 23423, int.MaxValue
                },
                f =>
                    Test(new FTestMessage
                    {
                        Fuint32 = f
                    }));
        }

        [Test]
        public void UInt64Ok()
        {
            Array.ForEach(
                new ulong[]
                {
                    123U, 0U, int.MaxValue, 23423U, long.MaxValue, ulong.MaxValue
                },
                f =>
                    Test(new FTestMessage
                    {
                        Fuint64 = f
                    }));
        }

        [Test]
        public void SInt32Ok()
        {
            Array.ForEach(
                new[]
                {
                    123, 0, -234, -int.MaxValue, 23423, int.MaxValue
                },
                f =>
                    Test(new FTestMessage
                    {
                        Fsint32 = f
                    }));
        }

        [Test]
        public void SInt64Ok()
        {
            Array.ForEach(
                new[]
                {
                    123, 0, -234, -int.MaxValue, 23423, int.MaxValue, long.MinValue, long.MaxValue
                },
                f =>
                    Test(new FTestMessage
                    {
                        Fsint64 = f
                    }));
        }

        [Test]
        public void Fixed32Ok()
        {
            Array.ForEach(
                new uint[]
                {
                    123, 0, uint.MaxValue, 23423, int.MaxValue
                },
                f =>
                    Test(new FTestMessage
                    {
                        Ffixed32 = f
                    }));
        }

        [Test]
        public void Fixed64Ok()
        {
            Array.ForEach(
                new ulong[]
                {
                    123U, 0U, int.MaxValue, 23423U, long.MaxValue, ulong.MaxValue
                },
                f =>
                    Test(new FTestMessage
                    {
                        Ffixed64 = f
                    }));
        }

        [Test]
        public void SFixed32Ok()
        {
            Array.ForEach(
                new[]
                {
                    123, 0, -234, -int.MaxValue, 23423, int.MaxValue
                },
                f =>
                    Test(new FTestMessage
                    {
                        Fsfixed32 = f
                    }));
        }

        [Test]
        public void SFixed64Ok()
        {
            Array.ForEach(
                new[]
                {
                    123, 0, -234, -int.MaxValue, 23423, int.MaxValue, long.MinValue, long.MaxValue
                },
                f =>
                    Test(new FTestMessage
                    {
                        Fsfixed64 = f
                    }));
        }

        [Test]
        public void BoolOk()
        {
            Array.ForEach(
                new bool?[]
                {
                    null, true, false
                },
                f =>
                    Test(new FTestMessage
                    {
                        Fbool = f
                    }));
        }

        [Test]
        public void StringOk()
        {
            Array.ForEach(
                new[]
                {
                    null, "test", "Привіт"
                },
                f =>
                    Test(new FTestMessage
                    {
                        Fstring = f
                    }));

            Test(new FTestMessage
            {
                Fstring = ""
            }, x => Assert.IsTrue(string.IsNullOrEmpty(x.Fstring)));
        }

        [Test]
        public void BytesOk()
        {
            Array.ForEach(
                new[]
                {
                    null,new byte[]{1}, new byte[]{1,2,3,4,5},   
                },
                f =>
                    Test(new FTestMessage
                    {
                        Fbytes = f
                    }));

            Test(new FTestMessage
            {
                Fbytes = new byte[0]
            }, x => Assert.IsTrue(x.Fbytes == null || x.Fbytes.Length == 0));
        }

        [Test]
        public void EnumOk()
        {
            Array.ForEach(
                new[]
                {
                    (TestEnum?)null, TestEnum.Value0, TestEnum.Value1, 
                },
                f =>
                    Test(new FTestMessage
                    {
                        Fenum = f
                    }));
        }

        [Test]
        public void InnerOk()
        {
            Array.ForEach(
                new[]
                {
                    null,
                    new InnerMessage(),
                    new InnerMessage{Test = 42},
                    new InnerMessage{Test = int.MaxValue},
                    new InnerMessage{Test = int.MinValue},
                },
                f =>
                    Test(new FTestMessage
                    {
                        Finner = f
                    }));
        }

        [Test]
        public void RepeatedInnerOk()
        {
            Array.ForEach(
                new[]
                {
                    null,
                    new List<InnerMessage>{ new InnerMessage{Test = 42}},
                    new List<InnerMessage>{ new InnerMessage(), new InnerMessage{Test = 42}},
                    new List<InnerMessage>{ new InnerMessage{Test = int.MinValue}, new InnerMessage(), new InnerMessage{Test = int.MaxValue}},

                },
                f =>
                    Test(new FTestMessage
                    {
                        Frep = f
                    }));
            Test(new FTestMessage{ Frep = new List<InnerMessage>() }, m => Assert.IsTrue(m.Frep == null || m.Frep.Count == 0));
        }

        [Test]
        public void RepeatedEnumOk()
        {
            Array.ForEach(
                new[]
                {
                    null,
                    new List<TestEnum>{ TestEnum.Value0},
                    new List<TestEnum>{ TestEnum.Value0, TestEnum.Value0},
                    new List<TestEnum>{ TestEnum.Value0, TestEnum.Value1, TestEnum.Value0, TestEnum.Value1},

                },
                f =>
                    Test(new FTestMessage
                    {
                        FrepEnum = f
                    }));
            Test(new FTestMessage { FrepEnum = new List<TestEnum>() }, m => Assert.IsTrue(m.FrepEnum == null || m.FrepEnum.Count == 0));
        }

        [Test]
        public void RepeatedStringOk()
        {
            Array.ForEach(
                new[]
                {
                    null,
                    new List<string>{ "test"},
                    new List<string>{ "test", "Привіт"},
                    new List<string>{ "This", "Is", "SPARTA!!!@!"}
                },
                f =>
                    Test(new FTestMessage
                    {
                        FrepString = f
                    }));
            Test(new FTestMessage { FrepString = new List<string>() }, m => Assert.IsTrue(m.FrepString == null || m.FrepString.Count == 0));
        }

        [Test]
        public void RepeatedFixed32Ok()
        {
            Array.ForEach(
                new[]
                {
                    null,
                    new List<uint>{ 1},
                    new List<uint>{ 0, 1, uint.MaxValue, uint.MinValue},
                    new List<uint>{ 1, 5, 6, int.MaxValue, uint.MaxValue}
                },
                f =>
                    Test(new FTestMessage
                    {
                        FrepFixed32 = f
                    }));
            Test(new FTestMessage { FrepFixed32 = new List<uint>() }, m => Assert.IsTrue(m.FrepFixed32 == null || m.FrepFixed32.Count == 0));
        }

        [Test]
        public void RepeatedUint32Ok()
        {
            Array.ForEach(
                new[]
                {
                    null,
                    new List<uint>{ 1},
                    new List<uint>{ 0, 1, uint.MaxValue, uint.MinValue},
                    new List<uint>{ 1, 5, 6, int.MaxValue, uint.MaxValue}
                },
                f =>
                    Test(new FTestMessage
                    {
                        FrepUint32 = f
                    }));
            Test(new FTestMessage { FrepUint32 = new List<uint>() }, m => Assert.IsTrue(m.FrepUint32 == null || m.FrepUint32.Count == 0));
        }
    }
}