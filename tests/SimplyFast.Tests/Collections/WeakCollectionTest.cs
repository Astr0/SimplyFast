using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using SF.Collections;

namespace SF.Tests.Collections
{
    [TestFixture]
    public class WeakCollectionTest
    {
        [Test]
        public void WeakCollectionFromEnumerable()
        {
            var items = Enumerable.Range(0, 10).Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var wc = new WeakCollection<string>(items);
            Assert.AreEqual(10, wc.CapCount);
            Assert.AreEqual(10, wc.Count);
            Assert.IsTrue(wc.SequenceEqual(items));
        }

        [Test]
        public void AddWorks()
        {
            var wc = new WeakCollection<string> {"test"};
            Assert.AreEqual("test", wc.First());
        }

        [Test]
        public void ClearWorks()
        {
            var wc = new WeakCollection<string> { "test" };
            wc.Clear();
            Assert.AreEqual(0, wc.Count);
            Assert.IsFalse(wc.Any());
        }

        [Test]
        public void ContainsWorks()
        {
            var arr = new[] {"a", "b", "c", "d"};
            var wc = new WeakCollection<string> (arr);
            Assert.IsTrue(wc.Contains("a"));
            Assert.IsTrue(wc.Contains("b"));
            Assert.IsTrue(wc.Contains("c"));
            Assert.IsTrue(wc.Contains("d"));
            Assert.IsFalse(wc.Contains("e"));
        }

        [Test]
        public void CopyToWorks()
        {
            var arr = new[] { "a", "b", "c", "d" };
            var wc = new WeakCollection<string>(arr);
            var target = new string[5];
            wc.CopyTo(target, 0);
            Assert.IsTrue(arr.SequenceEqual(target.Take(4)));
            Assert.IsNull(target[4]);
            target[0] = null;
            wc.CopyTo(target, 1);
            Assert.IsTrue(arr.SequenceEqual(target.Skip(1)));
        }

        [Test]
        public void RemoveWorks()
        {
            var arr = new[] { "a", "b", "c", "d", "c" };
            var wc = new WeakCollection<string>(arr);
            Assert.IsTrue(wc.Remove("c"));
            Assert.IsTrue(wc.SequenceEqual(new[]{"a", "b", "d", "c"}));
            Assert.IsTrue(wc.Remove("c"));
            Assert.IsTrue(wc.SequenceEqual(new[] { "a", "b", "d" }));
            Assert.IsFalse(wc.Remove("c"));
            Assert.IsTrue(wc.SequenceEqual(new[] { "a", "b", "d" }));
        }

        [Test]
        public void NotReadOnly()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var wc = new WeakCollection<string>();
            Assert.IsFalse(wc.IsReadOnly);
        }

        [Test]
        public void AddWorksAfterCollect()
        {
            var wc = new WeakCollection<string>();
            {
                wc.Add(new string('a', 1));
                Assert.AreEqual("a", wc.First());
            }
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.IsTrue(wc.CapCount <= 1);
            //Assert.AreEqual(0, wc.Count);
            wc.Add(new string('b', 1));
            Assert.AreEqual(1, wc.Count);
            Assert.AreEqual("b", wc.First());
        }

        [Test]
        public void ClearWorksAfterCollect()
        {
            var wc = new WeakCollection<string>();
            {
                var a = new string('a', 1);
                wc.Add(a);
                Assert.AreEqual("a", wc.First());
            }
            GC.Collect();
            GC.WaitForFullGCComplete();
            wc.Clear();
            Assert.AreEqual(0, wc.CapCount);
            Assert.IsFalse(wc.Any());
        }

        [Test]
        public void ContainsWorksAfterCollect()
        {
            var arr = new[] { "a", "b" };
            var wc = new WeakCollection<string>(arr);
            {
                wc.Add(new string('c', 1));
                wc.Add(new string('d', 1));
                Assert.IsTrue(wc.Contains("a"));
                Assert.IsTrue(wc.Contains("b"));
                Assert.IsTrue(wc.Contains("c"));
                Assert.IsTrue(wc.Contains("d"));
            }
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.IsTrue(wc.Contains("a"));
            Assert.IsTrue(wc.Contains("b"));
            Assert.IsFalse(wc.Contains("c"));
            Assert.IsFalse(wc.Contains("d"));
            Assert.IsFalse(wc.Contains("e"));
        }

        [Test]
        public void CopyToWorksAfterCollect()
        {
            var arr = new[] { "a", "b" };
            var wc = new WeakCollection<string>(arr);
            {
                wc.Add(new string('c', 1));
                wc.Add(new string('d', 1));
            }
            GC.Collect();
            GC.WaitForFullGCComplete();
            var target = new string[5];
            wc.CopyTo(target, 1);
            Assert.IsTrue(arr.SequenceEqual(target.Skip(1).Take(2)));
            Assert.IsNull(target[0]);
            Assert.IsNull(target[3]);
            Assert.IsNull(target[4]);
        }

        [Test]
        public void RemoveWorksAfterCollect()
        {
            var arr = new[] { "a", "b" };
            var wc = new WeakCollection<string>(arr);
            {
                wc.Add(new string('c', 1));
                wc.Add(new string('d', 1));
                wc.Add(new string('c', 1));
            }
            GC.Collect();
            GC.WaitForFullGCComplete();
            Assert.IsFalse(wc.Remove("c"));
            Assert.IsFalse(wc.Remove("d"));
            Assert.IsTrue(wc.SequenceEqual(new[] { "a", "b" }));
        }
    }
}