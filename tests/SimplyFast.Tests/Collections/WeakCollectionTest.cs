using System.Globalization;
using System.Linq;
using Xunit;
using SimplyFast.Collections;

namespace SimplyFast.Tests.Collections
{
    
    public class WeakCollectionTest
    {
        [Fact]
        public void WeakCollectionFromEnumerable()
        {
            var items = Enumerable.Range(0, 10).Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var wc = new WeakCollection<string>(items);
            Assert.Equal(10, wc.CapCount);
            Assert.Equal(10, wc.Count);
            Assert.True(wc.SequenceEqual(items));
        }

        [Fact]
        public void AddWorks()
        {
            var wc = new WeakCollection<string> {"test"};
            Assert.Equal("test", wc.First());
        }

        [Fact]
        public void ClearWorks()
        {
            var wc = new WeakCollection<string>(10) { "test" };
            wc.Clear();
            Assert.Equal(0, wc.Count);
            Assert.False(wc.Any());
        }

        [Fact]
        public void ContainsWorks()
        {
            var arr = new[] {"a", "b", "c", "d"};
            var wc = new WeakCollection<string> (arr);
            Assert.True(wc.Contains("a"));
            Assert.True(wc.Contains("b"));
            Assert.True(wc.Contains("c"));
            Assert.True(wc.Contains("d"));
            Assert.False(wc.Contains("e"));
        }

        [Fact]
        public void CopyToWorks()
        {
            var arr = new[] { "a", "b", "c", "d" };
            var wc = new WeakCollection<string>(arr);
            var target = new string[5];
            wc.CopyTo(target, 0);
            Assert.True(arr.SequenceEqual(target.Take(4)));
            Assert.Null(target[4]);
            target[0] = null;
            wc.CopyTo(target, 1);
            Assert.True(arr.SequenceEqual(target.Skip(1)));
        }

        [Fact]
        public void RemoveWorks()
        {
            var arr = new[] { "a", "b", "c", "d", "c" };
            var wc = new WeakCollection<string>(arr);
            Assert.True(wc.Remove("c"));
            Assert.True(wc.OrderBy(x => x).SequenceEqual(new[]{"a", "b", "c", "d" }));
            Assert.True(wc.Remove("c"));
            Assert.True(wc.OrderBy(x => x).SequenceEqual(new[] { "a", "b", "d" }));
            Assert.False(wc.Remove("c"));
            Assert.True(wc.OrderBy(x => x).SequenceEqual(new[] { "a", "b", "d" }));
        }

        [Fact]
        public void NotReadOnly()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var wc = new WeakCollection<string>();
            Assert.False(wc.IsReadOnly);
        }

        [Fact]
        public void AddWorksAfterCollect()
        {
            var wc = new WeakCollection<string>();
            {
                wc.Add(new string('a', 1));
                Assert.Equal("a", wc.First());
            }
            GCEx.CollectAndWait();
            Assert.True(wc.CapCount <= 1);
            //Assert.Equal(0, wc.Count);
            wc.Add(new string('b', 1));
            Assert.Equal(1, wc.Count);
            Assert.Equal("b", wc.First());
        }

        [Fact]
        public void ClearWorksAfterCollect()
        {
            var wc = new WeakCollection<string>();
            {
                var a = new string('a', 1);
                wc.Add(a);
                Assert.Equal("a", wc.First());
            }
            GCEx.CollectAndWait();
            wc.Clear();
            Assert.Equal(0, wc.CapCount);
            Assert.False(wc.Any());
        }

        [Fact]
        public void ContainsWorksAfterCollect()
        {
            var arr = new[] { "a", "b" };
            var wc = new WeakCollection<string>(arr);
            {
                wc.Add(new string('c', 1));
                wc.Add(new string('d', 1));
                Assert.True(wc.Contains("a"));
                Assert.True(wc.Contains("b"));
                Assert.True(wc.Contains("c"));
                Assert.True(wc.Contains("d"));
            }
            GCEx.CollectAndWait();
            Assert.True(wc.Contains("a"));
            Assert.True(wc.Contains("b"));
            Assert.False(wc.Contains("c"));
            Assert.False(wc.Contains("d"));
            Assert.False(wc.Contains("e"));
        }

        [Fact]
        public void CopyToWorksAfterCollect()
        {
            var arr = new[] { "a", "b" };
            var wc = new WeakCollection<string>(arr);
            {
                wc.Add(new string('c', 1));
                wc.Add(new string('d', 1));
            }
            GCEx.CollectAndWait();
            var target = new string[5];
            wc.CopyTo(target, 1);
            Assert.True(arr.SequenceEqual(target.Skip(1).Take(2)));
            Assert.Null(target[0]);
            Assert.Null(target[3]);
            Assert.Null(target[4]);
        }

        [Fact]
        public void RemoveWorksAfterCollect()
        {
            var arr = new[] { "a", "b" };
            var wc = new WeakCollection<string>(arr);
            {
                wc.Add(new string('c', 1));
                wc.Add(new string('d', 1));
                wc.Add(new string('c', 1));
            }
            GCEx.CollectAndWait();
            Assert.False(wc.Remove("c"));
            Assert.False(wc.Remove("d"));
            Assert.True(wc.SequenceEqual(new[] { "a", "b" }));
        }
    }
}