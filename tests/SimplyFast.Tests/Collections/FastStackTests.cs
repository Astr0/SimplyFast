using System;
using NUnit.Framework;
using SF.Collections;

namespace SF.Tests.Collections
{
    [TestFixture]
    public class FastStackTests
    {
        [Test]
        public static void WorksForValueTypes()
        {
            var c = new FastStack<int>();
            c.Push(1);
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual(1, c.Peek());
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual(1, c.Pop());
            Assert.AreEqual(0, c.Count);
            c.Push(1);
            c.Push(2);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(2, c.Peek());
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(2, c.Pop());
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual(1, c.Peek());
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual(1, c.Pop());
            Assert.AreEqual(0, c.Count);
        }

        [Test]
        public static void WorksForReferenceTypes()
        {
            var c = new FastStack<string>();
            c.Push("1");
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual("1", c.Peek());
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual("1", c.Pop());
            Assert.AreEqual(0, c.Count);
            c.Push("1");
            c.Push("2");
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual("2", c.Peek());
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual("2", c.Pop());
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual("1", c.Peek());
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual("1", c.Pop());
            Assert.AreEqual(0, c.Count);
        }

        [Test]
        public static void DoesntHoldReferences()
        {
            var wr1 = new WeakReference(new object());
            var wr2 = new WeakReference(new object());
            var c = new FastStack<object>();
            c.Push(wr1.Target);
            c.Push(wr2.Target);
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.AreEqual(2, c.Count);
            Assert.IsTrue(wr1.IsAlive);
            Assert.IsTrue(wr2.IsAlive);
            c.Pop();
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.AreEqual(1, c.Count);
            Assert.IsTrue(wr1.IsAlive);
            Assert.IsFalse(wr2.IsAlive);
            c.Pop();
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.AreEqual(0, c.Count);
            Assert.IsFalse(wr1.IsAlive);
            Assert.IsFalse(wr2.IsAlive);
        }
    }
}