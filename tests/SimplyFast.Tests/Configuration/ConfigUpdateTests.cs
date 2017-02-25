using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using SimplyFast.Configuration;

namespace SimplyFast.Tests.Configuration
{
    [TestFixture]
    public class ConfigUpdateTests
    {
        [SetUp]
        public void Setup()
        {
            _config = new DictionaryConfig();
        }

        private IConfig _config;

        [Test]
        public void UpdateFromArgs()
        {
            var args = new[] { "do_something", "-u", "test", "-p", "other" };
            _config.UpdateFromArgs(args, argsDelimiter: "|");
            Assert.AreEqual(string.Join("|", args), _config[ConfigUpdateEx.ArgsKey]);
        }

        [Test]
        public void UpdateFromConfOk()
        {
            _config.UpdateFromConf(new[]
            {
                "//comment",
                "test=1",
                "test2 = 2 and space"
            });
            Assert.AreEqual("1", _config["test"]);
            Assert.AreEqual("2 and space", _config["test2"]);

            _config.UpdateFromConf(new[]
            {
                "test=2",
                "//comment",
                "test3 =!@#!@#^^"
            });
            Assert.AreEqual("2", _config["test"]);
            Assert.AreEqual("2 and space", _config["test2"]);
            Assert.AreEqual("!@#!@#^^", _config["test3"]);
        }

        [Test]
        public void UpdateFromConfFileOk()
        {
            var tmp1 = Path.GetTempFileName();
            var tmp2 = Path.GetTempFileName();
            try
            {
                File.WriteAllLines(tmp1, new[]
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
                File.Delete(tmp1);
                File.Delete(tmp2);
            }
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
    }
}