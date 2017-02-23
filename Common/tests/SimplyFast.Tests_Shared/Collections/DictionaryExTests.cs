using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SF.Collections;

namespace SF.Tests.Collections
{
    [TestFixture]
    public class DictionaryExTests
    {
        [Test]
        public void GetOrDefaultIsAnExtensionMethod()
        {
            IDictionary<int, string> dictionary = new Dictionary<int, string> {{1, "test1"}, {2, "test2"}};
            Assert.AreEqual("test1", dictionary.GetOrDefault(1, "1"));
            Assert.AreEqual("test2", dictionary.GetOrDefault(2, "2"));
        }

        [Test]
        public void GetOrDefaultUsesDictionary()
        {
            var dictionary = new Dictionary<int, string> {{1, "test1"}, {2, "test2"}};
            Assert.AreEqual("test1", dictionary.GetOrDefault(1, "t"));
            Assert.AreEqual("test2", dictionary.GetOrDefault(2, "t"));
            dictionary[1] = "test1m";
            Assert.AreEqual("test1m", dictionary.GetOrDefault(1, "t"));
            Assert.AreEqual("t", dictionary.GetOrDefault(3, "t"));
            Assert.IsFalse(dictionary.ContainsKey(3));
        }

        [Test]
        public void GetOrDefaultWithDelegateIsAnExtensionMethod()
        {
            IDictionary<int, string> dictionary = new Dictionary<int, string> {{1, "test1"}, {2, "test2"}};
            Assert.AreEqual("test1", dictionary.GetOrAdd(1, x => "123"));
            Assert.AreEqual("test2", dictionary.GetOrAdd(2, x => "123"));
        }

        [Test]
        public void GetOrDefaultWithDelegateUsesDictionary()
        {
            var dictionary = new Dictionary<int, string> {{1, "test1"}, {2, "test2"}};
            Assert.AreEqual("test1", dictionary.GetOrAdd(1, x => "new"));
            Assert.AreEqual("test2", dictionary.GetOrAdd(2, x => "new"));
            dictionary[1] = "test1m";
            Assert.AreEqual("test1m", dictionary.GetOrAdd(1, x => "new"));
            dictionary.GetOrAdd(3, x => "new");
            Assert.AreEqual("new", dictionary[3]);
        }

        [Test]
        public void GetOrDefaultWithDelegateWorks()
        {
            var dictionary = new Dictionary<int, double> {{1, 2.5}, {2, 3}};
            Assert.AreEqual(2.5, dictionary.GetOrAdd(1, x => 1));
            Assert.AreEqual(3, dictionary.GetOrAdd(2, x => 1));
            Assert.AreEqual(1, dictionary.GetOrAdd(3, x => 1));
        }

        [Test]
        public void GetOrDefaultWorksWithReferenceTypes()
        {
            var dictionary = new Dictionary<int, object> {{1, 2.5}, {2, "test"}};
            Assert.AreEqual(2.5, dictionary.GetOrDefault(1, 3));
            Assert.AreEqual("test", dictionary.GetOrDefault(2, "t"));
            Assert.IsNull(dictionary.GetOrDefault(3, null));
        }

        [Test]
        public void GetOrDefaultWorksWithValueTypes()
        {
            var dictionary = new Dictionary<int, double> {{1, 2.5}, {2, 3}};
            Assert.AreEqual(2.5, dictionary.GetOrDefault(1, 2));
            Assert.AreEqual(3, dictionary.GetOrDefault(2, 2));
            Assert.AreEqual(5, dictionary.GetOrDefault(3, 5));
        }

        [Test]
        public void GetOrAddIsAnExtensionMethod()
        {
            IDictionary<int, double> dictionary = new Dictionary<int, double> {{1, 3}, {2, 4.5}};
            Assert.AreEqual(3, dictionary.GetOrAdd(1));
            Assert.AreEqual(4.5, dictionary.GetOrAdd(2));
            Assert.AreEqual(0, dictionary.GetOrAdd(3));
        }

        [Test]
        public void GetOrAddUsesDictionary()
        {
            var dictionary = new Dictionary<int, double> {{1, 3.5}, {2, 4.5}};
            Assert.AreEqual(3.5, dictionary.GetOrAdd(1));
            Assert.AreEqual(4.5, dictionary.GetOrAdd(2));
            dictionary[1] = 5.5;
            Assert.AreEqual(5.5, dictionary.GetOrAdd(1));
            dictionary.GetOrAdd(3);
            Assert.AreEqual(0, dictionary[3]);
        }

        [Test]
        public void GetOrAddWorksWithReferenceTypes()
        {
            var dictionary = new Dictionary<int, object> {{1, 2.5}, {2, "test"}};
            Assert.AreEqual(2.5, dictionary.GetOrAdd(1));
            Assert.AreEqual("test", dictionary.GetOrAdd(2));
            var newObj = dictionary.GetOrAdd(3);
            Assert.IsNotNull(newObj);
            Assert.IsInstanceOf(typeof (object), newObj);
        }

        [Test]
        public void GetOrAddWorksWithValueTypes()
        {
            var dictionary = new Dictionary<int, double> {{1, 2.5}, {2, 3}};
            Assert.AreEqual(2.5, dictionary.GetOrAdd(1));
            Assert.AreEqual(3, dictionary.GetOrAdd(2));
            Assert.AreEqual(0, dictionary.GetOrAdd(3));
        }

        [Test]
        public void GetOrDefaultWorks()
        {
            var dictionary = new Dictionary<int, object> {{1, 2.5}, {2, "test"}};
            Assert.AreEqual(2.5, dictionary.GetOrDefault(1));
            Assert.AreEqual("test", dictionary.GetOrDefault(2));
            Assert.AreEqual(null, dictionary.GetOrDefault(3));
            Assert.AreEqual(2, dictionary.Count);
        }

        [Test]
        public void GetOrAddConcurrentWorks()
        {
            var dictionary = new ConcurrentDictionary<int, object>(new Dictionary<int, object> { { 1, 2.5 }, { 2, "test" } });
            bool added;
            Assert.AreEqual(2.5, dictionary.GetOrAdd(1, c =>
            {
                throw new KeyNotFoundException();
            }, out added));
            Assert.IsFalse(added);
            Assert.AreEqual("test", dictionary.GetOrAdd(2, c =>
            {
                throw new KeyNotFoundException();
            }, out added));
            Assert.IsFalse(added);
            var newObj = dictionary.GetOrAdd(3, c => new object(), out added);
            Assert.IsNotNull(newObj);
            Assert.IsInstanceOf(typeof(object), newObj);
            Assert.IsTrue(added);
        }

        [Test]
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public void GetOrAddConcurrentWorksConcurrently()
        {
            var dictionary = new ConcurrentDictionary<int, int>();

            const int count = 1000;
            const int threadCount = 10;
            var threadAdded = new int[threadCount];
            var threadCreated = new int[threadCount];
            var threads = new List<Thread>(threadCount);
            var hadErrors = false;
            using (var start = new ManualResetEvent(false))
            using (var finish = new CountdownEvent(threadCount))
            {
                for (var t = 0; t < threadCount; ++t)
                {
                    var thread = new Thread(indexObj =>
                    {
                        var index = (int) indexObj;
                        start.WaitOne();
                        for (int i = 0; i < count; i++)
                        {
                            var result = dictionary.GetOrAdd(i, k =>
                            {
                                threadCreated[index]++;
                                return k;
                            }, out bool added);
                            if (added)
                                threadAdded[index]++;
                            if (result != i)
                                hadErrors = true;
                        }
                        finish.Signal();
                    });
                    thread.Start(t);
                    threads.Add(thread);
                }
                Assert.AreEqual(threadCount, threads.Count);
                start.Set();
                finish.Wait();
                Assert.IsFalse(hadErrors);
                Assert.AreEqual(count, dictionary.Count);
                Assert.IsTrue(Enumerable.Range(0, count).All(i => dictionary[i] == i));
                Assert.AreEqual(count, threadAdded.Sum());
                Assert.GreaterOrEqual(threadCreated.Sum(), count);
            }
        }
        
    }
}