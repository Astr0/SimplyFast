using System.Collections.Generic;
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
        public void GetOrNewIsAnExtensionMethod()
        {
            IDictionary<int, double> dictionary = new Dictionary<int, double> {{1, 3}, {2, 4.5}};
            Assert.AreEqual(3, dictionary.GetOrAdd(1));
            Assert.AreEqual(4.5, dictionary.GetOrAdd(2));
            Assert.AreEqual(0, dictionary.GetOrAdd(3));
        }

        [Test]
        public void GetOrNewUsesDictionary()
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
        public void GetOrNewWorksWithReferenceTypes()
        {
            var dictionary = new Dictionary<int, object> {{1, 2.5}, {2, "test"}};
            Assert.AreEqual(2.5, dictionary.GetOrAdd(1));
            Assert.AreEqual("test", dictionary.GetOrAdd(2));
            var newObj = dictionary.GetOrAdd(3);
            Assert.IsNotNull(newObj);
            Assert.IsInstanceOf(typeof (object), newObj);
        }

        [Test]
        public void GetOrNewWorksWithValueTypes()
        {
            var dictionary = new Dictionary<int, double> {{1, 2.5}, {2, 3}};
            Assert.AreEqual(2.5, dictionary.GetOrAdd(1));
            Assert.AreEqual(3, dictionary.GetOrAdd(2));
            Assert.AreEqual(0, dictionary.GetOrAdd(3));
        }

        [Test]
        public void GetOrNullWorks()
        {
            var dictionary = new Dictionary<int, object> {{1, 2.5}, {2, "test"}};
            Assert.AreEqual(2.5, dictionary.GetOrDefault(1));
            Assert.AreEqual("test", dictionary.GetOrDefault(2));
            Assert.AreEqual(null, dictionary.GetOrDefault(3));
            Assert.AreEqual(2, dictionary.Count);
        }
    }
}