using System;
using System.Collections.Generic;
using Xunit;
using SimplyFast.Collections;
using SimplyFast.Serialization.Tests.Protobuf.TestData;

namespace SimplyFast.Serialization.Tests.Protobuf
{
    public abstract class MessageTests
    {
        protected abstract void Test(FTestMessage message, Action<FTestMessage> customAssert = null);

        protected static void AssertDeserialized(FTestMessage message, FTestMessage deserialized,
            Action<FTestMessage> customAssert)
        {
            if (customAssert != null)
                customAssert(deserialized);
            else
                Assert.Equal(message, deserialized);
        }

        [Fact]
        public void EmptyOk()
        {
            Test(new FTestMessage());
        }

        [Fact]
        public void FloatOk()
        {
            new[]
            {
                123, 23423f, float.NaN, float.MaxValue, float.MinValue, float.Epsilon, float.NegativeInfinity,
                float.PositiveInfinity
            }.ForEach(f =>
                Test(new FTestMessage
                {
                    Ffloat = f
                }));
        }

        [Fact]
        public void Int32Ok()
        {
            new[]
            {
                123, 0, -234, -int.MaxValue, 23423, int.MaxValue
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Fint32 = f
                    }));
        }

        [Fact]
        public void Int64Ok()
        {
            new[]
            {
                123, 0, -234, -int.MaxValue, 23423, int.MaxValue, long.MinValue, long.MaxValue
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Fint64 = f
                    }));
        }

        [Fact]
        public void UInt32Ok()
        {
            new uint[]
            {
                123, 0, uint.MaxValue, 23423, int.MaxValue
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Fuint32 = f
                    }));
        }

        [Fact]
        public void UInt64Ok()
        {
            new ulong[]
            {
                123U, 0U, int.MaxValue, 23423U, long.MaxValue, ulong.MaxValue
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Fuint64 = f
                    }));
        }

        [Fact]
        public void SInt32Ok()
        {
            new[]
            {
                123, 0, -234, -int.MaxValue, 23423, int.MaxValue
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Fsint32 = f
                    }));
        }

        [Fact]
        public void SInt64Ok()
        {
            new[]
            {
                123, 0, -234, -int.MaxValue, 23423, int.MaxValue, long.MinValue, long.MaxValue
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Fsint64 = f
                    }));
        }

        [Fact]
        public void Fixed32Ok()
        {
            new uint[]
            {
                123, 0, uint.MaxValue, 23423, int.MaxValue
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Ffixed32 = f
                    }));
        }

        [Fact]
        public void Fixed64Ok()
        {
            new ulong[]
            {
                123U, 0U, int.MaxValue, 23423U, long.MaxValue, ulong.MaxValue
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Ffixed64 = f
                    }));
        }

        [Fact]
        public void SFixed32Ok()
        {
            new[]
            {
                123, 0, -234, -int.MaxValue, 23423, int.MaxValue
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Fsfixed32 = f
                    }));
        }

        [Fact]
        public void SFixed64Ok()
        {
            new[]
            {
                123, 0, -234, -int.MaxValue, 23423, int.MaxValue, long.MinValue, long.MaxValue
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Fsfixed64 = f
                    }));
        }

        [Fact]
        public void BoolOk()
        {
            new bool?[]
            {
                null, true, false
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Fbool = f
                    }));
        }

        [Fact]
        public void StringOk()
        {
            new[]
            {
                null, "test", "Привіт"
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Fstring = f
                    }));

            Test(new FTestMessage
            {
                Fstring = ""
            }, x => Assert.True(string.IsNullOrEmpty(x.Fstring)));
        }

        [Fact]
        public void BytesOk()
        {
            new[]
            {
                null, new byte[] {1}, new byte[] {1, 2, 3, 4, 5}
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Fbytes = f
                    }));

            Test(new FTestMessage
            {
                Fbytes = new byte[0]
            }, x => Assert.True(x.Fbytes == null || x.Fbytes.Length == 0));
        }

        [Fact]
        public void EnumOk()
        {
            new[]
            {
                (SomeEnum?) null, SomeEnum.Value0, SomeEnum.Value1
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Fenum = f
                    }));
        }

        [Fact]
        public void InnerOk()
        {
            new[]
            {
                null,
                new InnerMessage(),
                new InnerMessage {Test = 42},
                new InnerMessage {Test = int.MaxValue},
                new InnerMessage {Test = int.MinValue}
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Finner = f
                    }));
        }

        [Fact]
        public void RepeatedInnerOk()
        {
            new[]
            {
                null,
                new List<InnerMessage> {new InnerMessage {Test = 42}},
                new List<InnerMessage> {new InnerMessage(), new InnerMessage {Test = 42}},
                new List<InnerMessage>
                {
                    new InnerMessage {Test = int.MinValue},
                    new InnerMessage(),
                    new InnerMessage {Test = int.MaxValue}
                }
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        Frep = f
                    }));
            Test(new FTestMessage {Frep = new List<InnerMessage>()},
                m => Assert.True(m.Frep == null || m.Frep.Count == 0));
        }

        [Fact]
        public void RepeatedEnumOk()
        {
            new[]
            {
                null,
                new List<SomeEnum> {SomeEnum.Value0},
                new List<SomeEnum> {SomeEnum.Value0, SomeEnum.Value0},
                new List<SomeEnum> {SomeEnum.Value0, SomeEnum.Value1, SomeEnum.Value0, SomeEnum.Value1}
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        FrepEnum = f
                    }));
            Test(new FTestMessage {FrepEnum = new List<SomeEnum>()},
                m => Assert.True(m.FrepEnum == null || m.FrepEnum.Count == 0));
        }

        [Fact]
        public void RepeatedStringOk()
        {
            new[]
            {
                null,
                new List<string> {"test"},
                new List<string> {"test", "Привіт"},
                new List<string> {"This", "Is", "SPARTA!!!@!"}
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        FrepString = f
                    }));
            Test(new FTestMessage {FrepString = new List<string>()},
                m => Assert.True(m.FrepString == null || m.FrepString.Count == 0));
        }

        [Fact]
        public void RepeatedFixed32Ok()
        {
            new[]
            {
                null,
                new List<uint> {1},
                new List<uint> {0, 1, uint.MaxValue, uint.MinValue},
                new List<uint> {1, 5, 6, int.MaxValue, uint.MaxValue}
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        FrepFixed32 = f
                    }));
            Test(new FTestMessage {FrepFixed32 = new List<uint>()},
                m => Assert.True(m.FrepFixed32 == null || m.FrepFixed32.Count == 0));
        }

        [Fact]
        public void RepeatedUint32Ok()
        {
            new[]
            {
                null,
                new List<uint> {1},
                new List<uint> {0, 1, uint.MaxValue, uint.MinValue},
                new List<uint> {1, 5, 6, int.MaxValue, uint.MaxValue}
            }.ForEach(
                f =>
                    Test(new FTestMessage
                    {
                        FrepUint32 = f
                    }));
            Test(new FTestMessage {FrepUint32 = new List<uint>()},
                m => Assert.True(m.FrepUint32 == null || m.FrepUint32.Count == 0));
        }
    }
}