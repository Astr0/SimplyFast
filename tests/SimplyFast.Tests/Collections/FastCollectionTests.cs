using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SF.Collections;

namespace SF.Tests.Collections
{
    [TestFixture]
    public class FastCollectionTests
    {
        [Test]
        public void DefaultZeroCapacity()
        {
            var collection = new FastCollection<int>();
            Assert.IsFalse(collection.IsReadOnly);
            Assert.AreEqual(0, collection.Capacity);
            collection.Capacity = 10;
            Assert.AreEqual(10, collection.Capacity);
        }

        [Test]
        public void CanCreateWithCapacity()
        {
            Assert.AreEqual(10, new FastCollection<int>(10).Capacity);
            Assert.IsFalse(new FastCollection<int>(10).IsReadOnly);
            Assert.AreEqual(0, new FastCollection<int>(-4).Capacity);
        }

        [Test]
        public void AddRemoveWorks()
        {
            var c = new FastCollection<int> { 1, 2, 3, 4, 5 };
            Assert.IsTrue(c.SequenceEqual(new[] { 1, 2, 3, 4, 5 }));
            Assert.AreEqual(5, c.Count);
            Assert.GreaterOrEqual(c.Capacity, 5);
            Assert.IsTrue(c.Remove(5));
            Assert.IsTrue(c.All(new[] { 1, 2, 3, 4 }.Contains));
            Assert.AreEqual(4, c.Count);
            Assert.GreaterOrEqual(c.Capacity, 4);
            c.RemoveAt(3);
            Assert.IsTrue(c.All(new[] { 1, 2, 3 }.Contains));
            Assert.AreEqual(3, c.Count);
            Assert.GreaterOrEqual(c.Capacity, 3);
            Assert.IsTrue(c.Remove(1));
            Assert.IsTrue(c.All(new[] { 2, 3 }.Contains));
            Assert.AreEqual(2, c.Count);
            Assert.GreaterOrEqual(c.Capacity, 2);
            Assert.IsFalse(c.Remove(1));
            var oneEl = c[1];
            c.RemoveAt(0);
            Assert.AreEqual(oneEl, c[0]);
            Assert.AreEqual(1, c.Count);
            c.Add(4);
            Assert.IsTrue(c.All(new[] { oneEl, 4 }.Contains));
            Assert.AreEqual(2, c.Count);
            c.Clear();
            Assert.IsTrue(c.SequenceEqual(new int[0]));
            Assert.AreEqual(0, c.Count);
        }

        [Test]
        public void AddRangeEnumerableWorks()
        {
            var c = new FastCollection<int>();
            const int count = 10;
            var array = (IEnumerable<int>)Enumerable.Range(0, count).ToArray();
            c.AddRange(array, 10);
            Assert.AreEqual(count, c.Count);
            Assert.IsTrue(c.All(array.Contains));
            c.AddRange(array, 0);
            Assert.AreEqual(count, c.Count);
            Assert.IsTrue(c.All(array.Contains));
            c.Clear();
            Assert.AreEqual(0, c.Count);
            c.AddRange(array, 0);
            Assert.AreEqual(0, c.Count);
            c.AddRange(array, 2);
            Assert.AreEqual(2, c.Count);
            Assert.IsTrue(c.All(array.Take(2).Contains));
        }

        [Test]
        public void AddRangeArrayWorks()
        {
            var c = new FastCollection<int>();
            var array = Enumerable.Range(0, 10).ToArray();
            c.AddRange(array, 0, 10);
            Assert.AreEqual(array.Length, c.Count);
            Assert.IsTrue(c.All(array.Contains));
            c.AddRange(array, 5, 0);
            Assert.AreEqual(array.Length, c.Count);
            Assert.IsTrue(c.All(array.Contains));
            c.Clear();
            Assert.AreEqual(0, c.Count);
            c.AddRange(array, 0);
            Assert.AreEqual(0, c.Count);
            c.AddRange(array, 0, 2);
            Assert.AreEqual(2, c.Count);
            Assert.IsTrue(c.All(array.Take(2).Contains));
            c.AddRange(array, 2, 3);
            Assert.AreEqual(5, c.Count);
            Assert.IsTrue(c.All(array.Take(5).Contains));
        }

        [Test]
        public void IndexOfWorks()
        {
            var c = new FastCollection<int> { 1, 2, 3 };
            Assert.AreEqual(1, c[c.IndexOf(1)]);
            Assert.AreEqual(2, c[c.IndexOf(2)]);
            Assert.AreEqual(3, c[c.IndexOf(3)]);
            Assert.AreEqual(-1, c.IndexOf(4));
            c.Remove(1);
            Assert.AreEqual(2, c[c.IndexOf(2)]);
            Assert.AreEqual(3, c[c.IndexOf(3)]);
            Assert.AreEqual(-1, c.IndexOf(1));
            c.Clear();
            Assert.AreEqual(-1, c.IndexOf(1));
            Assert.AreEqual(-1, c.IndexOf(2));
            Assert.AreEqual(-1, c.IndexOf(3));
            c.Add(3);
            Assert.AreEqual(-1, c.IndexOf(1));
            Assert.AreEqual(-1, c.IndexOf(2));
            Assert.AreEqual(0, c.IndexOf(3));
            Assert.AreEqual(3, c[0]);
        }

        [Test]
        public void ContainsWorks()
        {
            var c = new FastCollection<int> { 1, 2, 3 };
            Assert.IsTrue(c.Contains(1));
            Assert.IsTrue(c.Contains(2));
            Assert.IsTrue(c.Contains(3));
            Assert.IsFalse(c.Contains(4));
            c.Remove(1);
            Assert.IsFalse(c.Contains(1));
            Assert.IsTrue(c.Contains(2));
            Assert.IsTrue(c.Contains(3));
            Assert.IsFalse(c.Contains(4));
            c.Clear();
            Assert.IsFalse(c.Contains(1));
            Assert.IsFalse(c.Contains(2));
            Assert.IsFalse(c.Contains(3));
            c.Add(3);
            Assert.IsFalse(c.Contains(1));
            Assert.IsFalse(c.Contains(2));
            Assert.IsTrue(c.Contains(3));
        }

        [Test]
        public void SetterWorks()
        {
            var c = new FastCollection<int> { 1 };
            Assert.AreEqual(1, c[0]);
            Assert.IsTrue(c.Contains(1));
            Assert.IsFalse(c.Contains(2));
            c[0] = 2;
            Assert.AreEqual(2, c[0]);
            Assert.IsFalse(c.Contains(1));
            Assert.IsTrue(c.Contains(2));
            c.Add(3);
            Assert.IsFalse(c.Contains(1));
            Assert.IsTrue(c.Contains(2));
            Assert.IsTrue(c.Contains(3));
            Assert.IsFalse(c.Contains(4));
            c[c.IndexOf(3)] = 4;
            Assert.IsFalse(c.Contains(1));
            Assert.IsTrue(c.Contains(2));
            Assert.IsFalse(c.Contains(3));
            Assert.IsTrue(c.Contains(4));
            c[c.IndexOf(4)] = 2;
            Assert.IsFalse(c.Contains(1));
            Assert.IsTrue(c.Contains(2));
            Assert.IsFalse(c.Contains(3));
            Assert.IsFalse(c.Contains(4));
            Assert.AreEqual(2, c.Count);
        }

        [Test]
        public void CopyToWorks()
        {
            var arr = Enumerable.Range(0, 10).ToArray();
            var c = new FastCollection<int>();
            c.CopyTo(arr, 0);
            Assert.IsTrue(arr.SequenceEqual(Enumerable.Range(0, 10)));
            c.Add(5);
            c.CopyTo(arr, 0);
            Assert.AreEqual(5, arr[0]);
            Assert.IsTrue(arr.Skip(1).SequenceEqual(Enumerable.Range(1, 9)));
            c.CopyTo(arr, 1);
            Assert.AreEqual(5, arr[0]);
            Assert.AreEqual(5, arr[1]);
            Assert.IsTrue(arr.Skip(2).SequenceEqual(Enumerable.Range(2, 8)));
            c.Add(6);
            c.CopyTo(arr, 0);
            Assert.AreEqual(5, arr[0]);
            Assert.AreEqual(6, arr[1]);
            Assert.IsTrue(arr.Skip(2).SequenceEqual(Enumerable.Range(2, 8)));
            c.CopyTo(arr, 1);
            Assert.AreEqual(5, arr[0]);
            Assert.AreEqual(5, arr[1]);
            Assert.AreEqual(6, arr[2]);
            Assert.IsTrue(arr.Skip(3).SequenceEqual(Enumerable.Range(3, 7)));
        }

        [Test]
        public void DoesntHoldReferences()
        {
            var wr1 = new WeakReference(new object());
            var wr2 = new WeakReference(new object());
            var c = new FastCollection<object> { wr1.Target, wr2.Target };
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.AreEqual(2, c.Count);
            Assert.IsTrue(wr1.IsAlive);
            Assert.IsTrue(wr2.IsAlive);
            c.Remove(wr1.Target);
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.AreEqual(1, c.Count);
            Assert.IsFalse(wr1.IsAlive);
            Assert.IsTrue(wr2.IsAlive);
            c.Clear();
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.AreEqual(0, c.Count);
            Assert.IsFalse(wr1.IsAlive);
            Assert.IsFalse(wr2.IsAlive);
        }

        [Test]
        public void RemoveAllWorks()
        {
            var c = new FastCollection<int> { 1, 2, 3, 4, 5 };
            c.RemoveAll(x => x % 2 == 0);
            Assert.AreEqual(3, c.Count);
            Assert.IsTrue(c.All(new[]{1, 3, 5}.Contains));
            c.RemoveAll(x => false);
            Assert.AreEqual(3, c.Count);
            Assert.IsTrue(c.All(new[] { 1, 3, 5 }.Contains));
            c.RemoveAll(x => true);
            Assert.AreEqual(0, c.Count);
            Assert.IsFalse(c.Any());
            c.RemoveAll(x => true);
            Assert.AreEqual(0, c.Count);
            Assert.IsFalse(c.Any());
        }

        [Test]
        public void DoesntHoldReferencesRemoveAll()
        {
            var wr1 = new WeakReference(new object());
            var wr2 = new WeakReference(new object());
            var c = new FastCollection<object> { wr1.Target, wr2.Target };
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.AreEqual(2, c.Count);
            Assert.IsTrue(wr1.IsAlive);
            Assert.IsTrue(wr2.IsAlive);
            c.RemoveAll(x => x == wr1.Target);
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.AreEqual(1, c.Count);
            Assert.IsFalse(wr1.IsAlive);
            Assert.IsTrue(wr2.IsAlive);
            c.RemoveAll(x => x == wr2.Target);
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.AreEqual(0, c.Count);
            Assert.IsFalse(wr1.IsAlive);
            Assert.IsFalse(wr2.IsAlive);
        }

    }
}
