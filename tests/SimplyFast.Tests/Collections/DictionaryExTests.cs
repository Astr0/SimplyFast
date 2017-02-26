using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Xunit;
using SimplyFast.Collections;

namespace SimplyFast.Tests.Collections
{
    
    public class DictionaryExTests
    {
        [Fact]
        public void GetOrDefaultIsAnExtensionMethod()
        {
            IDictionary<int, string> dictionary = new Dictionary<int, string> {{1, "test1"}, {2, "test2"}};
            Assert.Equal("test1", dictionary.GetOrDefault(1, "1"));
            Assert.Equal("test2", dictionary.GetOrDefault(2, "2"));
        }

        [Fact]
        public void GetOrDefaultUsesDictionary()
        {
            var dictionary = new Dictionary<int, string> {{1, "test1"}, {2, "test2"}};
            Assert.Equal("test1", dictionary.GetOrDefault(1, "t"));
            Assert.Equal("test2", dictionary.GetOrDefault(2, "t"));
            dictionary[1] = "test1m";
            Assert.Equal("test1m", dictionary.GetOrDefault(1, "t"));
            Assert.Equal("t", dictionary.GetOrDefault(3, "t"));
            Assert.False(dictionary.ContainsKey(3));
        }

        [Fact]
        public void GetOrDefaultWithDelegateIsAnExtensionMethod()
        {
            IDictionary<int, string> dictionary = new Dictionary<int, string> {{1, "test1"}, {2, "test2"}};
            Assert.Equal("test1", dictionary.GetOrAdd(1, x => "123"));
            Assert.Equal("test2", dictionary.GetOrAdd(2, x => "123"));
        }

        [Fact]
        public void GetOrDefaultWithDelegateUsesDictionary()
        {
            var dictionary = new Dictionary<int, string> {{1, "test1"}, {2, "test2"}};
            Assert.Equal("test1", dictionary.GetOrAdd(1, x => "new"));
            Assert.Equal("test2", dictionary.GetOrAdd(2, x => "new"));
            dictionary[1] = "test1m";
            Assert.Equal("test1m", dictionary.GetOrAdd(1, x => "new"));
            dictionary.GetOrAdd(3, x => "new");
            Assert.Equal("new", dictionary[3]);
        }

        [Fact]
        public void GetOrDefaultWithDelegateWorks()
        {
            var dictionary = new Dictionary<int, double> {{1, 2.5}, {2, 3}};
            Assert.Equal(2.5, dictionary.GetOrAdd(1, x => 1));
            Assert.Equal(3, dictionary.GetOrAdd(2, x => 1));
            Assert.Equal(1, dictionary.GetOrAdd(3, x => 1));
        }

        [Fact]
        public void GetOrDefaultWorksWithReferenceTypes()
        {
            var dictionary = new Dictionary<int, object> {{1, 2.5}, {2, "test"}};
            Assert.Equal(2.5, dictionary.GetOrDefault(1, 3));
            Assert.Equal("test", dictionary.GetOrDefault(2, "t"));
            Assert.Null(dictionary.GetOrDefault(3, null));
        }

        [Fact]
        public void GetOrDefaultWorksWithValueTypes()
        {
            var dictionary = new Dictionary<int, double> {{1, 2.5}, {2, 3}};
            Assert.Equal(2.5, dictionary.GetOrDefault(1, 2));
            Assert.Equal(3, dictionary.GetOrDefault(2, 2));
            Assert.Equal(5, dictionary.GetOrDefault(3, 5));
        }

        [Fact]
        public void GetOrAddIsAnExtensionMethod()
        {
            IDictionary<int, double> dictionary = new Dictionary<int, double> {{1, 3}, {2, 4.5}};
            Assert.Equal(3, dictionary.GetOrAdd(1));
            Assert.Equal(4.5, dictionary.GetOrAdd(2));
            Assert.Equal(0, dictionary.GetOrAdd(3));
        }

        [Fact]
        public void GetOrAddUsesDictionary()
        {
            var dictionary = new Dictionary<int, double> {{1, 3.5}, {2, 4.5}};
            Assert.Equal(3.5, dictionary.GetOrAdd(1));
            Assert.Equal(4.5, dictionary.GetOrAdd(2));
            dictionary[1] = 5.5;
            Assert.Equal(5.5, dictionary.GetOrAdd(1));
            dictionary.GetOrAdd(3);
            Assert.Equal(0, dictionary[3]);
        }

        [Fact]
        public void GetOrAddWorksWithReferenceTypes()
        {
            var dictionary = new Dictionary<int, object> {{1, 2.5}, {2, "test"}};
            Assert.Equal(2.5, dictionary.GetOrAdd(1));
            Assert.Equal("test", dictionary.GetOrAdd(2));
            var newObj = dictionary.GetOrAdd(3);
            Assert.NotNull(newObj);
            Assert.IsType(typeof (object), newObj);
        }

        [Fact]
        public void GetOrAddWorksWithValueTypes()
        {
            var dictionary = new Dictionary<int, double> {{1, 2.5}, {2, 3}};
            Assert.Equal(2.5, dictionary.GetOrAdd(1));
            Assert.Equal(3, dictionary.GetOrAdd(2));
            Assert.Equal(0, dictionary.GetOrAdd(3));
        }

        [Fact]
        public void GetOrDefaultWorks()
        {
            var dictionary = new Dictionary<int, object> {{1, 2.5}, {2, "test"}};
            Assert.Equal(2.5, dictionary.GetOrDefault(1));
            Assert.Equal("test", dictionary.GetOrDefault(2));
            Assert.Equal(null, dictionary.GetOrDefault(3));
            Assert.Equal(2, dictionary.Count);
        }

        [Fact]
        public void GetOrAddConcurrentWorks()
        {
            var dictionary = new ConcurrentDictionary<int, object>(new Dictionary<int, object> { { 1, 2.5 }, { 2, "test" } });
            bool added;
            Assert.Equal(2.5, dictionary.GetOrAdd(1, c =>
            {
                throw new KeyNotFoundException();
            }, out added));
            Assert.False(added);
            Assert.Equal("test", dictionary.GetOrAdd(2, c =>
            {
                throw new KeyNotFoundException();
            }, out added));
            Assert.False(added);
            var newObj = dictionary.GetOrAdd(3, c => new object(), out added);
            Assert.NotNull(newObj);
            Assert.IsType(typeof(object), newObj);
            Assert.True(added);
        }

        [Fact]
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
                Assert.Equal(threadCount, threads.Count);
                start.Set();
                finish.Wait();
                Assert.False(hadErrors);
                Assert.Equal(count, dictionary.Count);
                Assert.True(Enumerable.Range(0, count).All(i => dictionary[i] == i));
                Assert.Equal(count, threadAdded.Sum());
                Assert.InRange(threadCreated.Sum(), count, count * threadCount);
            }
        }
        
    }
}