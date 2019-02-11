using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using SimplyFast.IO;

namespace SimplyFast.Tests.IO
{
    
    public class FastBufferWriterTests
    {
        private byte[] _buffer;
        protected virtual FastBufferWriter Buf(int count)
        {
            _buffer = new byte[count];
            return new FastBufferWriter(_buffer);
        }

        protected virtual void AssertWritten(Action<FastBufferWriter> write, params byte[] bytes)
        {
            var writer = Buf(bytes.Length);
            write(writer);
            Assert.Equal(bytes.Length, writer.Index);
            AssertWritten(bytes);
            if (bytes.Length == 0)
                return;
            Assert.Throws<InvalidDataException>(() => write(writer));
            var writerFail = Buf(bytes.Length - 1);
            Assert.Throws<InvalidDataException>(() => write(writerFail));
        }

        protected virtual void AssertWritten(byte[] bytes)
        {
            Assert.True(_buffer.Take(bytes.Length).SequenceEqual(bytes));
        }

        [Fact]
        public void BytesOk()
        {
            AssertWritten(w => w.WriteByte(1), 1);
            AssertWritten(w => w.WriteBytes(1, 2), 1, 2);
            AssertWritten(w => w.WriteBytes(1, 2, 3), 1, 2, 3);
            AssertWritten(w => w.WriteBytes(1, 2, 3, 4), 1, 2, 3, 4);
            AssertWritten(w => w.WriteBytes(1, 2, 3, 4, 5), 1, 2, 3, 4, 5);
        }

        [Fact]
        public void LittleEndianOk()
        {
            AssertWritten(w => w.WriteLittleEndian32(324432U), BitConverter.GetBytes(324432U));
            AssertWritten(w => w.WriteLittleEndian32(332U), BitConverter.GetBytes(332U));
            AssertWritten(w => w.WriteLittleEndian32(43543556U), BitConverter.GetBytes(43543556U));
            AssertWritten(w => w.WriteLittleEndian32(0U), BitConverter.GetBytes(0U));
            AssertWritten(w => w.WriteLittleEndian32(uint.MaxValue), BitConverter.GetBytes(uint.MaxValue));

            AssertWritten(w => w.WriteLittleEndian64(324432UL), BitConverter.GetBytes(324432UL));
            AssertWritten(w => w.WriteLittleEndian64(332UL), BitConverter.GetBytes(332UL));
            AssertWritten(w => w.WriteLittleEndian64(43543556UL), BitConverter.GetBytes(43543556UL));
            AssertWritten(w => w.WriteLittleEndian64(43512312332443556UL), BitConverter.GetBytes(43512312332443556UL));
            AssertWritten(w => w.WriteLittleEndian64(8812692341213000300UL), BitConverter.GetBytes(8812692341213000300UL));
            AssertWritten(w => w.WriteLittleEndian64(0UL), BitConverter.GetBytes(0UL));
            AssertWritten(w => w.WriteLittleEndian64(ulong.MaxValue), BitConverter.GetBytes(ulong.MaxValue));
        }

        [Fact]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public void VarInt32SeemsOk()
        {
            AssertWritten(w => w.WriteVarInt32(0U), 0);
            AssertWritten(w => w.WriteVarInt32((1U << 7) - 1), 127);
            AssertWritten(w => w.WriteVarInt32(1U << 7), 128, 1);
            AssertWritten(w => w.WriteVarInt32((1U << 14) - 1), 255, 127);
            AssertWritten(w => w.WriteVarInt32(1U << 14), 128, 128, 1);
            AssertWritten(w => w.WriteVarInt32((1U << 21) - 1), 255, 255, 127);
            AssertWritten(w => w.WriteVarInt32(1U << 21), 128, 128, 128, 1);
            AssertWritten(w => w.WriteVarInt32((1U << 28) - 1), 255, 255, 255, 127);
            AssertWritten(w => w.WriteVarInt32(1U << 28), 128, 128, 128, 128, 1);
        }

        [Fact]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public void VarInt64SeemsOk()
        {
            AssertWritten(w => w.WriteVarInt64(0UL), 0);
            AssertWritten(w => w.WriteVarInt64((1UL << 7) - 1), 127);
            AssertWritten(w => w.WriteVarInt64(1UL << 7), 128, 1);
            AssertWritten(w => w.WriteVarInt64((1UL << 14) - 1), 255, 127);
            AssertWritten(w => w.WriteVarInt64(1UL << 14), 128, 128, 1);
            AssertWritten(w => w.WriteVarInt64((1UL << 21) - 1), 255, 255, 127);
            AssertWritten(w => w.WriteVarInt64(1UL << 21), 128, 128, 128, 1);
            AssertWritten(w => w.WriteVarInt64((1UL << 28) - 1), 255, 255, 255, 127);
            AssertWritten(w => w.WriteVarInt64(1UL << 28), 128, 128, 128, 128, 1);
            AssertWritten(w => w.WriteVarInt64((1UL << 35) - 1), 255, 255, 255, 255, 127);
            AssertWritten(w => w.WriteVarInt64(1UL << 35), 128, 128, 128, 128, 128, 1);
            AssertWritten(w => w.WriteVarInt64((1UL << 42) - 1), 255, 255, 255, 255, 255, 127);
            AssertWritten(w => w.WriteVarInt64(1UL << 42), 128, 128, 128, 128, 128, 128, 1);
            AssertWritten(w => w.WriteVarInt64((1UL << 49) - 1), 255, 255, 255, 255, 255, 255, 127);
            AssertWritten(w => w.WriteVarInt64(1UL << 49), 128, 128, 128, 128, 128, 128, 128, 1);
            AssertWritten(w => w.WriteVarInt64((1UL << 56) - 1), 255, 255, 255, 255, 255, 255, 255, 127);
            AssertWritten(w => w.WriteVarInt64(1UL << 56), 128, 128, 128, 128, 128, 128, 128, 128, 1);
            AssertWritten(w => w.WriteVarInt64((1UL << 63) - 1), 255, 255, 255, 255, 255, 255, 255, 255, 127);
            AssertWritten(w => w.WriteVarInt64(1UL << 63), 128, 128, 128, 128, 128, 128, 128, 128, 128, 1);
        }

        [Fact]
        public void FloatsOk()
        {
            AssertWritten(w => w.WriteLittleEndianFloat(0.234234F), BitConverter.GetBytes(0.234234F));
            AssertWritten(w => w.WriteLittleEndianFloat(5523423390023485F), BitConverter.GetBytes(5523423390023485F));
            AssertWritten(w => w.WriteLittleEndianFloat(2343240.234234F), BitConverter.GetBytes(2343240.234234F));
            AssertWritten(w => w.WriteLittleEndianFloat(123345989.24324234477f), BitConverter.GetBytes(123345989.24324234477f));
            AssertWritten(w => w.WriteLittleEndianFloat(float.Epsilon), BitConverter.GetBytes(float.Epsilon));
            AssertWritten(w => w.WriteLittleEndianFloat(float.MinValue), BitConverter.GetBytes(float.MinValue));
            AssertWritten(w => w.WriteLittleEndianFloat(float.MaxValue), BitConverter.GetBytes(float.MaxValue));
            AssertWritten(w => w.WriteLittleEndianFloat(float.NaN), BitConverter.GetBytes(float.NaN));
            AssertWritten(w => w.WriteLittleEndianFloat(float.NegativeInfinity), BitConverter.GetBytes(float.NegativeInfinity));
            AssertWritten(w => w.WriteLittleEndianFloat(float.PositiveInfinity), BitConverter.GetBytes(float.PositiveInfinity));
            AssertWritten(w => w.WriteLittleEndianFloat(0f), BitConverter.GetBytes(0f));
        }

        [Fact]
        public void DoubleOk()
        {
            AssertWritten(w => w.WriteDouble(0.234234d), BitConverter.GetBytes(0.234234d));
            AssertWritten(w => w.WriteDouble(5523423390023485d), BitConverter.GetBytes(5523423390023485d));
            AssertWritten(w => w.WriteDouble(2343240.234234d), BitConverter.GetBytes(2343240.234234d));
            AssertWritten(w => w.WriteDouble(123345989.24324234477d), BitConverter.GetBytes(123345989.24324234477d));
            AssertWritten(w => w.WriteDouble(double.Epsilon), BitConverter.GetBytes(double.Epsilon));
            AssertWritten(w => w.WriteDouble(double.MinValue), BitConverter.GetBytes(double.MinValue));
            AssertWritten(w => w.WriteDouble(double.MaxValue), BitConverter.GetBytes(double.MaxValue));
            AssertWritten(w => w.WriteDouble(double.NaN), BitConverter.GetBytes(double.NaN));
            AssertWritten(w => w.WriteDouble(double.NegativeInfinity), BitConverter.GetBytes(double.NegativeInfinity));
            AssertWritten(w => w.WriteDouble(double.PositiveInfinity), BitConverter.GetBytes(double.PositiveInfinity));
            AssertWritten(w => w.WriteDouble(0d), BitConverter.GetBytes(0d));
        }

        [Fact]
        public void StringSizeOk()
        {
            Assert.Equal(5, FastBufferWriter.GetUtf8StringSize("testy"));
            Assert.Equal(12, FastBufferWriter.GetUtf8StringSize("Привіт"));
        }

        [Fact]
        public void StringWriteOk()
        {
            AssertWritten(w => w.WriteRawUtf8String(""));
            AssertWritten(w => w.WriteRawUtf8String("testy"), Encoding.UTF8.GetBytes("testy"));
            AssertWritten(w => w.WriteRawUtf8String("Привіт"), Encoding.UTF8.GetBytes("Привіт"));
        }

        [Fact]
        public void BytesWriteOk()
        {
            AssertWritten(w => w.WriteRawBytes(new byte[0]));
            AssertWritten(w => w.WriteRawBytes(new byte[] { 1 }), 1);
            AssertWritten(w => w.WriteRawBytes(new byte[] { 1, 2, 3, 4, 5, 6, 7 }), 1, 2, 3, 4, 5, 6, 7);
        }

        [Fact]
        public void CaptureOkStart()
        {
            var buf = Buf(10);
            var pos = buf.CaptureCurrentPosition();
            Assert.Throws<InvalidOperationException>(() => buf.CaptureCurrentPosition(1));
            buf.WriteBytes(1, 2, 3, 4, 5);
            AssertWritten(new byte[]{1, 2, 3, 4, 5});
            Assert.Equal(1, buf.Peek(pos));
            Assert.Equal(2, buf.Peek(pos, 1));
            Assert.Equal(3, buf.Peek(pos, 2));
            Assert.Equal(4, buf.Peek(pos, 3));
            Assert.Equal(5, buf.Peek(pos, 4));
            Assert.Throws<InvalidOperationException>(() => buf.Peek(pos, -1));
            Assert.Throws<InvalidOperationException>(() => buf.Peek(pos, 6));
            buf.WriteCopy(pos, 5);
            AssertWritten(new byte[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 });
            Assert.Throws<InvalidDataException>(() => buf.WriteCopy(pos, 1));
        }

        [Fact]
        public void CaptureOkEnd()
        {
            var buf = Buf(10);
            buf.WriteBytes(1, 2, 3, 4, 5);
            var pos = buf.CaptureCurrentPosition(5);
            Assert.Throws<InvalidOperationException>(() => buf.CaptureCurrentPosition(6));
            AssertWritten(new byte[] { 1, 2, 3, 4, 5 });
            Assert.Equal(1, buf.Peek(pos));
            Assert.Equal(2, buf.Peek(pos, 1));
            Assert.Equal(3, buf.Peek(pos, 2));
            Assert.Equal(4, buf.Peek(pos, 3));
            Assert.Equal(5, buf.Peek(pos, 4));
            Assert.Throws<InvalidOperationException>(() => buf.Peek(pos, -1));
            Assert.Throws<InvalidOperationException>(() => buf.Peek(pos, 6));
            buf.WriteCopy(pos, 5);
            AssertWritten(new byte[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 });
            Assert.Throws<InvalidDataException>(() => buf.WriteCopy(pos, 1));
        }

        [Fact]
        public void CaptureOkMiddle()
        {
            var buf = Buf(10);
            buf.WriteBytes(1, 2, 3, 4, 5);
            var pos = buf.CaptureCurrentPosition();
            Assert.Throws<InvalidOperationException>(() => buf.CaptureCurrentPosition(11));
            buf.WriteBytes(1, 2, 3, 4, 5);
            AssertWritten(new byte[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5});
            Assert.Equal(1, buf.Peek(pos));
            Assert.Equal(2, buf.Peek(pos, 1));
            Assert.Equal(3, buf.Peek(pos, 2));
            Assert.Equal(4, buf.Peek(pos, 3));
            Assert.Equal(5, buf.Peek(pos, 4));

            Assert.Equal(1, buf.Peek(pos, -5));
            Assert.Equal(2, buf.Peek(pos, -4));
            Assert.Equal(3, buf.Peek(pos, -3));
            Assert.Equal(4, buf.Peek(pos, -2));
            Assert.Equal(5, buf.Peek(pos, -1));
            Assert.Throws<InvalidOperationException>(() => buf.Peek(pos, -6));
            Assert.Throws<InvalidOperationException>(() => buf.Peek(pos, 6));
            Assert.Throws<InvalidDataException>(() => buf.WriteCopy(pos, 1));
        }
    }
}