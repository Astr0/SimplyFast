using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using System.Linq;
using SimplyFast.Collections;

namespace SimplyFast.Tests.Collections
{
    
    public class FastCollectionTests
    {
        [Fact]
        public void DefaultZeroCapacity()
        {
            var collection = new FastCollection<int>();
            Assert.False(collection.IsReadOnly);
            Assert.Equal(0, collection.Capacity);
            collection.Capacity = 10;
            Assert.Equal(10, collection.Capacity);
        }

        [Fact]
        public void CanCreateWithCapacity()
        {
            Assert.Equal(10, new FastCollection<int>(10).Capacity);
            Assert.False(new FastCollection<int>(10).IsReadOnly);
            Assert.Equal(0, new FastCollection<int>(-4).Capacity);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
        private static void AssertEquals<T>(FastCollection<T> collection, IEnumerable<T> enumerable)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();
            Assert.Equal(array.Length, collection.Count);
            Assert.True(collection.All(e => array.Contains(e)));
        }

        [Fact]
        public void AddRemoveWorks()
        {
            var c = new FastCollection<int> { 1, 2, 3, 4, 5 };
            Assert.True(c.SequenceEqual(new[] { 1, 2, 3, 4, 5 }));
            Assert.Equal(5, c.Count);
            var originalCapacity = c.Capacity;
            Assert.InRange(originalCapacity, 5, int.MaxValue);
            Assert.True(c.Remove(5));
            AssertEquals(c, new[] { 1, 2, 3, 4 });
            Assert.InRange(c.Capacity, 4, originalCapacity);
            c.RemoveAt(3);
            AssertEquals(c, new[] { 1, 2, 3 });
            Assert.InRange(c.Capacity, 3, originalCapacity);
            Assert.True(c.Remove(1));
            AssertEquals(c, new[] { 2, 3 });
            Assert.InRange(c.Capacity, 2, originalCapacity);
            Assert.False(c.Remove(1));
            var oneEl = c[1];
            c.RemoveAt(0);
            Assert.Equal(oneEl, c[0]);
            Assert.Equal(1, c.Count);
            c.Add(4);
            AssertEquals(c, new[] { oneEl, 4 });
            c.Clear();
            Assert.True(c.SequenceEqual(new int[0]));
            Assert.Equal(0, c.Count);
        }

        [Fact]
        public void AddRangeEnumerableWorks()
        {
            var c = new FastCollection<int>();
            const int count = 10;
            var array = (IEnumerable<int>)Enumerable.Range(0, count).ToArray();
            c.AddRange(array, 10);
            AssertEquals(c, array);
            c.AddRange(array, 0);
            AssertEquals(c, array);
            c.Clear();
            Assert.Equal(0, c.Count);
            c.AddRange(array, 0);
            Assert.Equal(0, c.Count);
            c.AddRange(array, 2);
            AssertEquals(c, array.Take(2));
        }

        [Fact]
        public void AddRangeArrayWorks()
        {
            var c = new FastCollection<int>();
            var array = Enumerable.Range(0, 10).ToArray();
            c.AddRange(array, 0, 10);
            AssertEquals(c, array);
            c.AddRange(array, 5, 0);
            AssertEquals(c, array);
            c.Clear();
            Assert.Equal(0, c.Count);
            c.AddRange(array, 0);
            Assert.Equal(0, c.Count);
            c.AddRange(array, 0, 2);
            AssertEquals(c, array.Take(2));
            c.AddRange(array, 2, 3);
            AssertEquals(c, array.Take(5));
        }

        [Fact]
        public void IndexOfWorks()
        {
            var c = new FastCollection<int> { 1, 2, 3 };
            Assert.Equal(1, c[c.IndexOf(1)]);
            Assert.Equal(2, c[c.IndexOf(2)]);
            Assert.Equal(3, c[c.IndexOf(3)]);
            Assert.Equal(-1, c.IndexOf(4));
            c.Remove(1);
            Assert.Equal(2, c[c.IndexOf(2)]);
            Assert.Equal(3, c[c.IndexOf(3)]);
            Assert.Equal(-1, c.IndexOf(1));
            c.Clear();
            Assert.Equal(-1, c.IndexOf(1));
            Assert.Equal(-1, c.IndexOf(2));
            Assert.Equal(-1, c.IndexOf(3));
            c.Add(3);
            Assert.Equal(-1, c.IndexOf(1));
            Assert.Equal(-1, c.IndexOf(2));
            Assert.Equal(0, c.IndexOf(3));
            Assert.Equal(3, c[0]);
        }

        [Fact]
        public void ContainsWorks()
        {
            var c = new FastCollection<int> { 1, 2, 3 };
            Assert.True(c.Contains(1));
            Assert.True(c.Contains(2));
            Assert.True(c.Contains(3));
            Assert.False(c.Contains(4));
            c.Remove(1);
            Assert.False(c.Contains(1));
            Assert.True(c.Contains(2));
            Assert.True(c.Contains(3));
            Assert.False(c.Contains(4));
            c.Clear();
            Assert.False(c.Contains(1));
            Assert.False(c.Contains(2));
            Assert.False(c.Contains(3));
            c.Add(3);
            Assert.False(c.Contains(1));
            Assert.False(c.Contains(2));
            Assert.True(c.Contains(3));
        }

        [Fact]
        public void SetterWorks()
        {
            var c = new FastCollection<int> { 1 };
            Assert.Equal(1, c[0]);
            Assert.True(c.Contains(1));
            Assert.False(c.Contains(2));
            c[0] = 2;
            Assert.Equal(2, c[0]);
            Assert.False(c.Contains(1));
            Assert.True(c.Contains(2));
            c.Add(3);
            Assert.False(c.Contains(1));
            Assert.True(c.Contains(2));
            Assert.True(c.Contains(3));
            Assert.False(c.Contains(4));
            c[c.IndexOf(3)] = 4;
            Assert.False(c.Contains(1));
            Assert.True(c.Contains(2));
            Assert.False(c.Contains(3));
            Assert.True(c.Contains(4));
            c[c.IndexOf(4)] = 2;
            Assert.False(c.Contains(1));
            Assert.True(c.Contains(2));
            Assert.False(c.Contains(3));
            Assert.False(c.Contains(4));
            Assert.Equal(2, c.Count);
        }

        [Fact]
        public void CopyToWorks()
        {
            var arr = Enumerable.Range(0, 10).ToArray();
            var c = new FastCollection<int>();
            c.CopyTo(arr, 0);
            Assert.True(arr.SequenceEqual(Enumerable.Range(0, 10)));
            c.Add(5);
            c.CopyTo(arr, 0);
            Assert.Equal(5, arr[0]);
            Assert.True(arr.Skip(1).SequenceEqual(Enumerable.Range(1, 9)));
            c.CopyTo(arr, 1);
            Assert.Equal(5, arr[0]);
            Assert.Equal(5, arr[1]);
            Assert.True(arr.Skip(2).SequenceEqual(Enumerable.Range(2, 8)));
            c.Add(6);
            c.CopyTo(arr, 0);
            Assert.Equal(5, arr[0]);
            Assert.Equal(6, arr[1]);
            Assert.True(arr.Skip(2).SequenceEqual(Enumerable.Range(2, 8)));
            c.CopyTo(arr, 1);
            Assert.Equal(5, arr[0]);
            Assert.Equal(5, arr[1]);
            Assert.Equal(6, arr[2]);
            Assert.True(arr.Skip(3).SequenceEqual(Enumerable.Range(3, 7)));
        }

        [Fact]
        public void DoesntHoldReferences()
        {
            var wr1 = new WeakReference(new object());
            var wr2 = new WeakReference(new object());
            var c = new FastCollection<object> { wr1.Target, wr2.Target };
            GC.Collect();
            //GC.WaitForFullGCComplete();
            Assert.Equal(2, c.Count);
            Assert.True(wr1.IsAlive);
            Assert.True(wr2.IsAlive);
            c.Remove(wr1.Target);
            GC.Collect();
            //GC.WaitForFullGCComplete();
            Assert.Equal(1, c.Count);
            Assert.False(wr1.IsAlive);
            Assert.True(wr2.IsAlive);
            c.Clear();
            GC.Collect();
            //GC.WaitForFullGCComplete();
            Assert.Equal(0, c.Count);
            Assert.False(wr1.IsAlive);
            Assert.False(wr2.IsAlive);
        }

        [Fact]
        public void EnumerableCreateWorks()
        {
            var range = Enumerable.Range(0, 10);
            var arr = Enumerable.Range(0, 10).ToArray();
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.True(arr.SequenceEqual(new FastCollection<int>(range).OrderBy(x => x)));
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.True(arr.SequenceEqual(new FastCollection<int>(arr).OrderBy(x => x)));
        }


        [Fact]
        public void RemoveAllWorks()
        {
            var c = new FastCollection<int> { 1, 2, 3, 4, 5 };
            c.RemoveAll(x => x % 2 == 0);
            AssertEquals(c, new[] { 1, 3, 5 });
            c.RemoveAll(x => false);
            AssertEquals(c, new[] { 1, 3, 5 });
            c.RemoveAll(x => true);
            Assert.Equal(0, c.Count);
            Assert.False(c.Any());
            c.RemoveAll(x => true);
            Assert.Equal(0, c.Count);
            Assert.False(c.Any());
        }

        [Fact]
        public void DoesntHoldReferencesRemoveAll()
        {
            var wr1 = new WeakReference(new object());
            var wr2 = new WeakReference(new object());
            var c = new FastCollection<object> { wr1.Target, wr2.Target };
            GCEx.CollectAndWait();
            Assert.Equal(2, c.Count);
            Assert.True(wr1.IsAlive);
            Assert.True(wr2.IsAlive);
            c.RemoveAll(x => x == wr1.Target);
            GCEx.CollectAndWait();
            Assert.Equal(1, c.Count);
            Assert.False(wr1.IsAlive);
            Assert.True(wr2.IsAlive);
            c.RemoveAll(x => x == wr2.Target);
            GCEx.CollectAndWait();
            Assert.Equal(0, c.Count);
            Assert.False(wr1.IsAlive);
            Assert.False(wr2.IsAlive);
        }

    }
}
