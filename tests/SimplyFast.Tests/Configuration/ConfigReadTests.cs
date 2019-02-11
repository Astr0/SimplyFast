using System;
using System.Linq;
using Xunit;
using SimplyFast.Configuration;

namespace SimplyFast.Tests.Configuration
{
    
    public class ConfigReadTests
    {
        private readonly IConfig _config;
        private readonly IReadOnlyConfig _readConfig;

        public ConfigReadTests()
        {
            _config = new DictionaryConfig
            {
                ["space"] = " ",
                ["empty"] = ""
            };
            _readConfig = _config;
        }

        
        [Fact]
        public void ReadArgs()
        {
            var args = new[] {"do_something", "-u", "test", "-p", "other"};
            _config.UpdateFromArgs(args, argsDelimiter: "|");
            var read = _readConfig.GetArgs(argsDelimiter: "|");
            Assert.True(read.SequenceEqual(args));
        }

        [Fact]
        public void ReadString()
        {
            _config["test"] = "test";
            Assert.Equal(null, _readConfig.GetString("null"));
            Assert.Equal(" ", _readConfig.GetString("space"));
            Assert.Equal("", _readConfig.GetString("empty"));
            Assert.Equal("test", _readConfig.GetString("test"));
        }

        [Fact]
        public void ReadInt32()
        {
            _config["test"] = "test";
            _config["value"] = "11";
            Assert.Equal(null, _readConfig.GetInt32("null"));
            Assert.Equal(null, _readConfig.GetInt32("space"));
            Assert.Equal(null, _readConfig.GetInt32("empty"));
            Assert.Throws<FormatException>(() => _readConfig.GetInt32("test"));
            Assert.Equal(11, _readConfig.GetInt32("value"));
        }

        [Fact]
        public void ReadInt64()
        {
            _config["test"] = "test";
            _config["value"] = "11";
            Assert.Equal(null, _readConfig.GetInt64("null"));
            Assert.Equal(null, _readConfig.GetInt64("space"));
            Assert.Equal(null, _readConfig.GetInt64("empty"));
            Assert.Throws<FormatException>(() => _readConfig.GetInt64("test"));
            Assert.Equal(11, _readConfig.GetInt64("value"));
        }

        private enum SomeEnum
        {
            SomeValue = 12
        }

        [Fact]
        public void ReadEnum()
        {
            _config["test"] = "test";
            _config["value"] = "12";
            _config["value2"] = "SomeValue";
            Assert.Equal(null, _readConfig.GetEnum<SomeEnum>("null"));
            Assert.Equal(null, _readConfig.GetEnum<SomeEnum>("space"));
            Assert.Equal(null, _readConfig.GetEnum<SomeEnum>("empty"));
            Assert.Throws<ArgumentException>(() => _readConfig.GetEnum<SomeEnum>("test"));
            Assert.Equal(SomeEnum.SomeValue, _readConfig.GetEnum<SomeEnum>("value"));
            Assert.Equal(SomeEnum.SomeValue, _readConfig.GetEnum<SomeEnum>("value2"));
        }

        [Fact]
        public void ReadTimeSpan()
        {
            _config["test"] = "test";
            _config["value"] = TimeSpan.FromHours(1.5).ToString("g");
            _config["value2"] = TimeSpan.FromDays(365).ToString("c");
            Assert.Equal(null, _readConfig.GetTimeSpan("null"));
            Assert.Equal(null, _readConfig.GetTimeSpan("space"));
            Assert.Equal(null, _readConfig.GetTimeSpan("empty"));
            Assert.Throws<FormatException>(() => _readConfig.GetTimeSpan("test"));
            Assert.Equal(TimeSpan.FromHours(1.5), _readConfig.GetTimeSpan("value"));
            Assert.Equal(TimeSpan.FromDays(365), _readConfig.GetTimeSpan("value2"));
        }

        [Fact]
        public void ReadBool()
        {
            _config["test"] = "test";
            _config["value"] = "true";
            _config["value2"] = "false";
            Assert.Equal(null, _readConfig.GetBool("null"));
            Assert.Equal(null, _readConfig.GetBool("space"));
            Assert.Equal(null, _readConfig.GetBool("empty"));
            Assert.Throws<FormatException>(() => _readConfig.GetBool("test"));
            Assert.Equal(true, _readConfig.GetBool("value"));
            Assert.Equal(false, _readConfig.GetBool("value2"));
            _config["value3"] = "t";
            _config["value4"] = "f";
            Assert.Equal(true, _readConfig.GetBool("value3"));
            Assert.Equal(false, _readConfig.GetBool("value4"));
            _config["value5"] = "1";
            _config["value6"] = "0";
            Assert.Equal(true, _readConfig.GetBool("value5"));
            Assert.Equal(false, _readConfig.GetBool("value6"));
        }
    }
}