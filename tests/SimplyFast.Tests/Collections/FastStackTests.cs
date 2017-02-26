using System;
using Xunit;
using SimplyFast.Collections;

namespace SimplyFast.Tests.Collections
{
    
    public class FastStackTests
    {
        [Fact]
        public static void WorksForValueTypes()
        {
            var c = new FastStack<int>();
            c.Push(1);
            Assert.Equal(1, c.Count);
            Assert.Equal(1, c.Peek());
            Assert.Equal(1, c.Count);
            Assert.Equal(1, c.Pop());
            Assert.Equal(0, c.Count);
            c.Push(1);
            c.Push(2);
            Assert.Equal(2, c.Count);
            Assert.Equal(2, c.Peek());
            Assert.Equal(2, c.Count);
            Assert.Equal(2, c.Pop());
            Assert.Equal(1, c.Count);
            Assert.Equal(1, c.Peek());
            Assert.Equal(1, c.Count);
            Assert.Equal(1, c.Pop());
            Assert.Equal(0, c.Count);
        }

        [Fact]
        public static void WorksForReferenceTypes()
        {
            var c = new FastStack<string>();
            c.Push("1");
            Assert.Equal(1, c.Count);
            Assert.Equal("1", c.Peek());
            Assert.Equal(1, c.Count);
            Assert.Equal("1", c.Pop());
            Assert.Equal(0, c.Count);
            c.Push("1");
            c.Push("2");
            Assert.Equal(2, c.Count);
            Assert.Equal("2", c.Peek());
            Assert.Equal(2, c.Count);
            Assert.Equal("2", c.Pop());
            Assert.Equal(1, c.Count);
            Assert.Equal("1", c.Peek());
            Assert.Equal(1, c.Count);
            Assert.Equal("1", c.Pop());
            Assert.Equal(0, c.Count);
        }

        [Fact]
        public static void DoesntHoldReferences()
        {
            var wr1 = new WeakReference(new object());
            var wr2 = new WeakReference(new object());
            var c = new FastStack<object>();
            c.Push(wr1.Target);
            c.Push(wr2.Target);
            GCEx.CollectAndWait();
            Assert.Equal(2, c.Count);
            Assert.True(wr1.IsAlive);
            Assert.True(wr2.IsAlive);
            c.Pop();
            GCEx.CollectAndWait();
            Assert.Equal(1, c.Count);
            Assert.True(wr1.IsAlive);
            Assert.False(wr2.IsAlive);
            c.Pop();
            GCEx.CollectAndWait();
            Assert.Equal(0, c.Count);
            Assert.False(wr1.IsAlive);
            Assert.False(wr2.IsAlive);
        }
    }
}