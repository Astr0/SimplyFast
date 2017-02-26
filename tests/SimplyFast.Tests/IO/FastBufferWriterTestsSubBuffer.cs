using System;
using System.IO;
using System.Linq;
using Xunit;
using SimplyFast.IO;

namespace SimplyFast.Tests.IO
{
    
    public class FastBufferWriterTestsSubBuffer : FastBufferWriterTests
    {
        private byte[] _buffer;
        protected override FastBufferWriter Buf(int count)
        {
            _buffer = new byte[count + 10];
            for (var i = 0; i < _buffer.Length; i++)
            {
                _buffer[i] = (byte) (i % 256);
            }
            return new FastBufferWriter(_buffer, 5, count);
        }

        protected override void AssertWritten(Action<FastBufferWriter> write, params byte[] bytes)
        {
            var writer = Buf(bytes.Length);
            write(writer);
            Assert.Equal(bytes.Length, writer.Index - 5);
            AssertWritten(bytes);
            if (bytes.Length == 0)
                return;
            Assert.Throws<InvalidDataException>(() => write(writer));
            var writerFail = Buf(bytes.Length - 1);
            Assert.Throws<InvalidDataException>(() => write(writerFail));
        }

        protected override void AssertWritten(byte[] bytes)
        {
            Assert.True(_buffer.Take(5).SequenceEqual(new byte []{0, 1, 2, 3, 4}));
            Assert.True(_buffer.Skip(5).Take(bytes.Length).SequenceEqual(bytes));
            var start = bytes.Length + 5;
            Assert.True(_buffer.Skip(start).SequenceEqual(Enumerable.Range(start, _buffer.Length - start).Select(i => (byte)(i % 256))));
        }
    }
}