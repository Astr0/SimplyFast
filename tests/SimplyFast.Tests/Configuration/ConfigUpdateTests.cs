using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using NUnit.Framework;
using SF.Configuration;

namespace SF.Tests.Configuration
{
    [TestFixture]
    public class ConfigUpdateTests
    {
        private IConfig _config;

        [SetUp]
        public void Setup()
        {
            _config = new DictionaryConfig();
        }

        [Test]
        public void UpdateFromKeyValuePairsOk()
        {
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test", "1"},
                {"test2", "2"}
            });
            Assert.AreEqual("1", _config["test"]);
            Assert.AreEqual("2", _config["test2"]);
            Assert.IsNull(_config["test3"]);
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test2", "_2"},
                {"test3", "3"}
            });
            Assert.AreEqual("1", _config["test"]);
            Assert.AreEqual("_2", _config["test2"]);
            Assert.AreEqual("3", _config["test3"]);
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test2", "2"},
                {"test3", null}
            });
            Assert.AreEqual("1", _config["test"]);
            Assert.AreEqual("2", _config["test2"]);
            Assert.AreEqual("3", _config["test3"]);
        }

        [Test]
        public void UpdateFromKeyValuePairsMapOk()
        {
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test", "1"},
                {"test2", "2"}
            }, k => "_" + k, v => v + "_");
            Assert.AreEqual("1_", _config["_test"]);
            Assert.AreEqual("2_", _config["_test2"]);
            Assert.IsNull(_config["_test3"]);
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test2", "_2"},
                {"test3", "3"}
            }, k => k == "test3" ? "test" : k, v => v.Trim('_'));
            Assert.AreEqual("1_", _config["_test"]);
            Assert.AreEqual("2_", _config["_test2"]);
            Assert.AreEqual("2", _config["test2"]);
            Assert.AreEqual("3", _config["test"]);
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test2", "_2"},
                {"_test", "2"}
            }, k => null);
            Assert.AreEqual("1_", _config["_test"]);
            Assert.AreEqual("2_", _config["_test2"]);
            Assert.AreEqual("2", _config["test2"]);
            Assert.AreEqual("3", _config["test"]);
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test2", "_2"},
                {"_test", "2"}
            }, k => k, v => null);
            Assert.AreEqual("1_", _config["_test"]);
            Assert.AreEqual("2_", _config["_test2"]);
            Assert.AreEqual("2", _config["test2"]);
            Assert.AreEqual("3", _config["test"]);
        }

        [Test]
        public void UpdateFromNameValueOk()
        {
            _config.UpdateFromNameValueCollection(new NameValueCollection
            {
                {"test", "1"},
                {"test2", "2"}
            }, k => "_" + k, v => v + "_");
            Assert.AreEqual("1_", _config["_test"]);
            Assert.AreEqual("2_", _config["_test2"]);
            Assert.IsNull(_config["_test3"]);
            _config.UpdateFromNameValueCollection(new NameValueCollection
            {
                {"_test", "1"},
                {"test2", "2"},
                {"test", "3"}
            });
            Assert.AreEqual("1", _config["_test"]);
            Assert.AreEqual("2_", _config["_test2"]);
            Assert.AreEqual("2", _config["test2"]);
            Assert.AreEqual("3", _config["test"]);
            _config.UpdateFromNameValueCollection(new NameValueCollection
            {
                {"test2", "_2"},
                {"_test", "2"}
            }, k => null);
            Assert.AreEqual("1", _config["_test"]);
            Assert.AreEqual("2_", _config["_test2"]);
            Assert.AreEqual("2", _config["test2"]);
            Assert.AreEqual("3", _config["test"]);
            _config.UpdateFromNameValueCollection(new NameValueCollection
            {
                {"test2", "_2"},
                {"_test", "2"}
            }, k => k, v => null);
            Assert.AreEqual("1", _config["_test"]);
            Assert.AreEqual("2_", _config["_test2"]);
            Assert.AreEqual("2", _config["test2"]);
            Assert.AreEqual("3", _config["test"]);
        }

        [Test]
        public void UpdateFromConfOk()
        {
            var tmp1 = Path.GetTempFileName();
            var tmp2 = Path.GetTempFileName();
            try
            {
                File.WriteAllLines(tmp1, new []
                {
                    "//comment",
                    "test=1",
                    "test2 = 2 and space"
                });
                _config.UpdateFromConf(tmp1);
                Assert.AreEqual("1", _config["test"]);
                Assert.AreEqual("2 and space", _config["test2"]);
                File.WriteAllLines(tmp2, new[]
{
                    "test=2",
                    "//comment",
                    "test3 =!@#!@#^^"
                });
                _config.UpdateFromConf(tmp2);
                Assert.AreEqual("2", _config["test"]);
                Assert.AreEqual("2 and space", _config["test2"]);
                Assert.AreEqual("!@#!@#^^", _config["test3"]);
            }
            finally
            {
                if (File.Exists(tmp1))
                    File.Delete(tmp1);
                if (File.Exists(tmp2))
                    File.Delete(tmp2);
            }
        }

        [Test]
        public void UpdateFromArgs()
        {
            var args = new[] {"do_something", "-u", "test", "-p", "other"};
            _config.UpdateFromArgs(args, argsDelimiter: "|");
            Assert.AreEqual(string.Join("|", args), _config[ConfigUpdateEx.ArgsKey]);
        }
    }
}