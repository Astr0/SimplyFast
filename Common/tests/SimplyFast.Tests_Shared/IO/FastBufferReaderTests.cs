using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SF.IO;

namespace SF.Tests.IO
{
    [TestFixture]
    public class FastBufferReaderTests
    {
        private byte[] _buffer;
        protected virtual FastBufferReader Buf(params byte[] bytes)
        {
            _buffer = new byte[bytes.Length];
            Array.Copy(bytes, _buffer, bytes.Length);
            return new FastBufferReader(_buffer);
        }

        protected virtual void AssertRead<T>(Func<FastBufferReader, T> read, T expected, Func<T, byte[]> getBytes)
        {
            var bytes = getBytes(expected);
            var reader = Buf(bytes);
            var readValue = read(reader);
            Assert.IsTrue(reader.End);
            Assert.AreEqual(expected, readValue);
            if (bytes.Length == 0)
                return;
            Assert.Throws<InvalidDataException>(() => read(reader));
            var readFail = Buf(bytes.Take(bytes.Length - 1).ToArray());
            Assert.Throws<InvalidDataException>(() => read(readFail));
        }

        [Test]
        public void SkipBytesOk()
        {
            var buf = Buf(1, 2, 3);
            buf.SkipBytes(1);
            Assert.IsFalse(buf.End);
            buf.SkipBytes(2);
            Assert.IsTrue(buf.End);
            Assert.Throws<InvalidDataException>(() => buf.SkipBytes(1));
        }

        [Test]
        public void DoubleOk()
        {
            AssertRead(r => r.ReadDouble(), 324234.324d, BitConverter.GetBytes);
            AssertRead(r => r.ReadDouble(), 0.3240023804808d, BitConverter.GetBytes);
            AssertRead(r => r.ReadDouble(), 0d, BitConverter.GetBytes);
            AssertRead(r => r.ReadDouble(), 23423234239732423489896622345d, BitConverter.GetBytes);
            AssertRead(r => r.ReadDouble(), double.MaxValue, BitConverter.GetBytes);
            AssertRead(r => r.ReadDouble(), double.MinValue, BitConverter.GetBytes);
            AssertRead(r => r.ReadDouble(), double.Epsilon, BitConverter.GetBytes);
            AssertRead(r => r.ReadDouble(), double.NaN, BitConverter.GetBytes);
            AssertRead(r => r.ReadDouble(), double.PositiveInfinity, BitConverter.GetBytes);
            AssertRead(r => r.ReadDouble(), double.NegativeInfinity, BitConverter.GetBytes);
        }

        [Test]
        public void FloatOk()
        {
            AssertRead(r => r.ReadLittleEndianFloat(), 324234.324f, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndianFloat(), 0.3240023804808f, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndianFloat(), 0f, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndianFloat(), 23423234239732423489896622345f, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndianFloat(), float.MaxValue, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndianFloat(), float.MinValue, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndianFloat(), float.Epsilon, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndianFloat(), float.NaN, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndianFloat(), float.PositiveInfinity, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndianFloat(), float.NegativeInfinity, BitConverter.GetBytes);
        }

        [Test]
        public void LittleEndian32Ok()
        {
            AssertRead(r => r.ReadLittleEndian32(), 0U, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndian32(), 3454798798U, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndian32(), 12313U, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndian32(), 1313234234U, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndian32(), uint.MaxValue, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndian32(), uint.MinValue, BitConverter.GetBytes);
        }

        [Test]
        public void LittleEndian64Ok()
        {
            AssertRead(r => r.ReadLittleEndian64(), 0UL, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndian64(), 34543583798798UL, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndian64(), 12313UL, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndian64(), 12313234234UL, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndian64(), 5645643412313234234UL, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndian64(), ulong.MaxValue, BitConverter.GetBytes);
            AssertRead(r => r.ReadLittleEndian64(), ulong.MinValue, BitConverter.GetBytes);
        }

        [Test]
        public void SkipVarIntOk()
        {
            var buf = Buf(VarIntHelper.GetVarInt32Bytes(889863845U));
            buf.SkipVarInt();
            Assert.IsTrue(buf.End);
            buf = Buf(VarIntHelper.GetVarInt64Bytes(ulong.MaxValue));
            buf.SkipVarInt();
            Assert.IsTrue(buf.End);
            Assert.Throws<InvalidDataException>(() => buf.SkipVarInt());
            buf = Buf(VarIntHelper.GetVarInt64Bytes(ulong.MaxValue).TakeWhile(x => x > 128).ToArray());
            Assert.Throws<InvalidDataException>(() => buf.SkipVarInt());
            buf = Buf(Enumerable.Range(0, 20).Select(x => (byte)128).ToArray());
            Assert.Throws<InvalidDataException>(() => buf.SkipVarInt());
        }

        [Test]
        public void ReadVarInt32Ok()
        {
            var buf = Buf(Enumerable.Range(0, 20).Select(x => (byte)128).ToArray());
            Assert.Throws<InvalidDataException>(() => buf.ReadVarInt32());
            for (var i = 0; i < 5; i++)
            {
                AssertRead(r => r.ReadVarInt32(), (1U << (7 * i)) - 1, VarIntHelper.GetVarInt32Bytes);
                AssertRead(r => r.ReadVarInt32(), 1U << (7 * i), VarIntHelper.GetVarInt32Bytes);
            }
            AssertRead(r => r.ReadVarInt32(), uint.MaxValue, VarIntHelper.GetVarInt32Bytes);
        }

        [Test]
        public void ReadVarInt64Ok()
        {
            var buf = Buf(Enumerable.Range(0, 20).Select(x => (byte)128).ToArray());
            Assert.Throws<InvalidDataException>(() => buf.ReadVarInt32());
            for (var i = 0; i < 10; i++)
            {
                AssertRead(r => r.ReadVarInt64(), (1UL << (7 * i)) - 1, VarIntHelper.GetVarInt64Bytes);
                AssertRead(r => r.ReadVarInt64(), 1UL << (7 * i), VarIntHelper.GetVarInt64Bytes);
            }
            AssertRead(r => r.ReadVarInt64(), ulong.MaxValue, VarIntHelper.GetVarInt64Bytes);
        }

        [Test]
        public void ReadByteOk()
        {
            AssertRead(r => r.ReadByte(), (byte)0, x => new[] { x });
            AssertRead(r => r.ReadByte(), (byte)127, x => new[] { x });
            AssertRead(r => r.ReadByte(), (byte)255, x => new[] { x });
        }

        [Test]
        public void ReadRawBytesOk()
        {
            AssertRead(r => r.ReadRawBytes(0), new byte[0], x => x);
            AssertRead(r => r.ReadRawBytes(1), new byte[] { 1 }, x => x);
            AssertRead(r => r.ReadRawBytes(5), new byte[] { 1, 2, 3, 4, 5 }, x => x);
        }

        [Test]
        public void ReadRawUtf8StringOk()
        {
            AssertRead(r => r.ReadRawUtf8String(0), "", x => new byte[0]);
            AssertRead(r => r.ReadRawUtf8String(5), "testy", Encoding.UTF8.GetBytes);
            AssertRead(r => r.ReadRawUtf8String(12), "Привіт", Encoding.UTF8.GetBytes);
        }
    }
}