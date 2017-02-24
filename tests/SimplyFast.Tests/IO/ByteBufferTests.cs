using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using SimplyFast.IO;

namespace SimplyFast.Tests.IO
{
    [TestFixture]
    public class ByteBufferTests
    {
        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CtorOk()
        {
            var buf = new ByteBuffer();
            Assert.AreEqual(0, buf.Buffer.Length);
            Assert.AreEqual(0, buf.BufferLength);
            Assert.AreEqual(0, buf.Offset);
            Assert.AreEqual(0, buf.Count);

            var arr = new byte[10];
            buf = new ByteBuffer(arr, 2, 5);
            Assert.AreEqual(arr, buf.Buffer);
            Assert.AreEqual(10, buf.Buffer.Length);
            Assert.AreEqual(10, buf.BufferLength);
            Assert.AreEqual(2, buf.Offset);
            Assert.AreEqual(5, buf.Count);

            Assert.DoesNotThrow(() => new ByteBuffer(null, 0, 0));
            Assert.DoesNotThrow(() => new ByteBuffer(new byte[10], 10, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ByteBuffer(null, 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ByteBuffer(new byte[10], -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ByteBuffer(new byte[10], 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ByteBuffer(new byte[10], 0, 11));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ByteBuffer(new byte[10], 1, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ByteBuffer(new byte[10], 11, -5));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ByteBuffer(new byte[10], -5, 20));
        }

        [Test]
        public void SetViewOk()
        {
            var buf = new ByteBuffer();
            Assert.DoesNotThrow(() => buf.SetView(0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => buf.SetView(1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => buf.SetView(0, 1));
            buf = new ByteBuffer(new byte[10], 0, 10);
            buf.SetView(2, 5);
            Assert.AreEqual(10, buf.BufferLength);
            Assert.AreEqual(2, buf.Offset);
            Assert.AreEqual(5, buf.Count);
            Assert.Throws<ArgumentOutOfRangeException>(() => buf.SetView(-1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => buf.SetView(0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => buf.SetView(0, 11));
            Assert.Throws<ArgumentOutOfRangeException>(() => buf.SetView(1, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => buf.SetView(11, -5));
            Assert.Throws<ArgumentOutOfRangeException>(() => buf.SetView(-5, 20));
        }

        [Test]
        public void ResetOk()
        {
            var arr = new byte[10];
            var buf = new ByteBuffer(arr, 0, 1);
            Assert.AreEqual(arr, buf.Buffer);
            Assert.AreEqual(10, buf.BufferLength);
            Assert.AreEqual(0, buf.Offset);
            Assert.AreEqual(1, buf.Count);
            buf.Reset(5);
            Assert.AreEqual(arr, buf.Buffer);
            Assert.AreEqual(10, buf.BufferLength);
            Assert.AreEqual(0, buf.Offset);
            Assert.AreEqual(0, buf.Count);
            buf.SetView(1, 3);
            buf.Reset(10);
            Assert.AreEqual(arr, buf.Buffer);
            Assert.AreEqual(10, buf.BufferLength);
            Assert.AreEqual(0, buf.Offset);
            Assert.AreEqual(0, buf.Count);
            buf.SetView(1, 3);
            buf.Reset(20);
            Assert.AreNotEqual(arr, buf.Buffer);
            Assert.AreEqual(20, buf.BufferLength);
            Assert.AreEqual(0, buf.Offset);
            Assert.AreEqual(0, buf.Count);
        }

        [Test]
        public void GrowOk()
        {
            var arr = Enumerable.Range(0, 10).Select(x => (byte)x).ToArray();
            var buf = new ByteBuffer(arr, 1, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => buf.Grow(5));
            Assert.Throws<ArgumentOutOfRangeException>(() => buf.Grow(-1));
            buf.Grow(20);
            Assert.AreEqual(20, buf.BufferLength);
            Assert.AreNotEqual(arr, buf.Buffer);
            Assert.AreEqual(1, buf.Offset);
            Assert.AreEqual(2, buf.Count);
            Assert.IsTrue(arr.SequenceEqual(buf.Buffer.Take(10)));
        }

        [Test]
        public void GrowEmptyOk()
        {
            var buf = new ByteBuffer();
            Assert.AreEqual(0, buf.BufferLength);
            buf.Grow();
            Assert.Greater(buf.BufferLength, 0);

        }
    }
}