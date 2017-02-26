using System.Collections.Generic;
using System.IO;
using Xunit;
using SimplyFast.Configuration;

namespace SimplyFast.Tests.Configuration
{
    
    public class ConfigUpdateTests
    {
        public ConfigUpdateTests()
        {
            _config = new DictionaryConfig();
        }

        private readonly IConfig _config;

        [Fact]
        public void UpdateFromArgs()
        {
            var args = new[] { "do_something", "-u", "test", "-p", "other" };
            _config.UpdateFromArgs(args, argsDelimiter: "|");
            Assert.Equal(string.Join("|", args), _config[ConfigUpdateEx.ArgsKey]);
        }

        [Fact]
        public void UpdateFromConfOk()
        {
            _config.UpdateFromConfLines(new[]
            {
                "//comment",
                "test=1",
                "test2 = 2 and space"
            });
            Assert.Equal("1", _config["test"]);
            Assert.Equal("2 and space", _config["test2"]);

            _config.UpdateFromConfLines(new[]
            {
                "test=2",
                "//comment",
                "test3 =!@#!@#^^"
            });
            Assert.Equal("2", _config["test"]);
            Assert.Equal("2 and space", _config["test2"]);
            Assert.Equal("!@#!@#^^", _config["test3"]);
        }

#if FILES
        [Fact]
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
                Assert.Equal("1", _config["test"]);
                Assert.Equal("2 and space", _config["test2"]);


                File.WriteAllLines(tmp2, new[]
                {
                    "test=2",
                    "//comment",
                    "test3 =!@#!@#^^"
                });
                _config.UpdateFromConf(tmp2);
                Assert.Equal("2", _config["test"]);
                Assert.Equal("2 and space", _config["test2"]);
                Assert.Equal("!@#!@#^^", _config["test3"]);
            }
            finally
            {
                File.Delete(tmp1);
                File.Delete(tmp2);
            }
        }
#endif

        [Fact]
        public void UpdateFromKeyValuePairsMapOk()
        {
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test", "1"},
                {"test2", "2"}
            }, k => "_" + k, v => v + "_");
            Assert.Equal("1_", _config["_test"]);
            Assert.Equal("2_", _config["_test2"]);
            Assert.Null(_config["_test3"]);
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test2", "_2"},
                {"test3", "3"}
            }, k => k == "test3" ? "test" : k, v => v.Trim('_'));
            Assert.Equal("1_", _config["_test"]);
            Assert.Equal("2_", _config["_test2"]);
            Assert.Equal("2", _config["test2"]);
            Assert.Equal("3", _config["test"]);
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test2", "_2"},
                {"_test", "2"}
            }, k => null);
            Assert.Equal("1_", _config["_test"]);
            Assert.Equal("2_", _config["_test2"]);
            Assert.Equal("2", _config["test2"]);
            Assert.Equal("3", _config["test"]);
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test2", "_2"},
                {"_test", "2"}
            }, k => k, v => null);
            Assert.Equal("1_", _config["_test"]);
            Assert.Equal("2_", _config["_test2"]);
            Assert.Equal("2", _config["test2"]);
            Assert.Equal("3", _config["test"]);
        }

        [Fact]
        public void UpdateFromKeyValuePairsOk()
        {
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test", "1"},
                {"test2", "2"}
            });
            Assert.Equal("1", _config["test"]);
            Assert.Equal("2", _config["test2"]);
            Assert.Null(_config["test3"]);
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test2", "_2"},
                {"test3", "3"}
            });
            Assert.Equal("1", _config["test"]);
            Assert.Equal("_2", _config["test2"]);
            Assert.Equal("3", _config["test3"]);
            _config.UpdateFromKeyValuePairs(new Dictionary<string, string>
            {
                {"test2", "2"},
                {"test3", null}
            });
            Assert.Equal("1", _config["test"]);
            Assert.Equal("2", _config["test2"]);
            Assert.Equal("3", _config["test3"]);
        }
    }
}