using System;
using System.IO;
using System.Linq;
using Xunit;
using SimplyFast.IO;

namespace SimplyFast.Tests.IO
{
    
    public class FastBufferReaderTestsView: FastBufferReaderTests
    {
        private byte[] _buffer;
        protected override FastBufferReader Buf(params byte[] bytes)
        {
            _buffer = new byte[bytes.Length + 10];
            Array.Copy(bytes, _buffer, bytes.Length);
            var rdr = new FastBufferReader(_buffer);
            rdr.SetView(bytes.Length);
            return rdr;
        }

        protected override void AssertRead<T>(Func<FastBufferReader, T> read, T expected, Func<T, byte[]> getBytes)
        {
            var bytes = getBytes(expected);
            var reader = Buf(bytes);
            var readValue = read(reader);
            Assert.True(reader.End);
            Assert.Equal(expected, readValue);
            if (bytes.Length == 0)
                return;
            Assert.Throws<InvalidDataException>(() => read(reader));
            var readFail = Buf(bytes);
            readFail.SetView(bytes.Length - 1);
            Assert.Throws<InvalidDataException>(() => read(readFail));
        }

        [Fact]
        public void SetRestoreViewWorks()
        {
            const ulong value = 43543543UL;
            var rdr = new FastBufferReader(BitConverter.GetBytes(value).Concat(BitConverter.GetBytes(value)).ToArray());
            var view = rdr.SetView(8);
            var subView = rdr.SetView(0);
            Assert.True(rdr.End);
            rdr.RestoreView(subView);
            Assert.Equal(value, rdr.ReadLittleEndian64());
            rdr.RestoreView(view);
            Assert.Equal(value, rdr.ReadLittleEndian64());
            Assert.Throws<InvalidOperationException>(() => rdr.RestoreView(subView));
        }
    }
}