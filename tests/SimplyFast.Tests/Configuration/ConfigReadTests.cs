using System;
using System.Linq;
using NUnit.Framework;
using SF.Configuration;

namespace SF.Tests.Configuration
{
    [TestFixture]
    public class ConfigReadTests
    {
        private IConfig _config;
        private IReadOnlyConfig _readConfig;

        [SetUp]
        public void Setup()
        {
            _config = new DictionaryConfig
            {
                ["space"] = " ",
                ["empty"] = ""
            };
            _readConfig = _config;
        }

        
        [Test]
        public void ReadArgs()
        {
            var args = new[] {"do_something", "-u", "test", "-p", "other"};
            _config.UpdateFromArgs(args, argsDelimiter: "|");
            var read = _readConfig.GetArgs(argsDelimiter: "|");
            Assert.IsTrue(read.SequenceEqual(args));
        }

        [Test]
        public void ReadString()
        {
            _config["test"] = "test";
            Assert.AreEqual(null, _readConfig.GetString("null"));
            Assert.AreEqual(" ", _readConfig.GetString("space"));
            Assert.AreEqual("", _readConfig.GetString("empty"));
            Assert.AreEqual("test", _readConfig.GetString("test"));
        }

        [Test]
        public void ReadInt32()
        {
            _config["test"] = "test";
            _config["value"] = "11";
            Assert.AreEqual(null, _readConfig.GetInt32("null"));
            Assert.AreEqual(null, _readConfig.GetInt32("space"));
            Assert.AreEqual(null, _readConfig.GetInt32("empty"));
            Assert.Throws<FormatException>(() => _readConfig.GetInt32("test"));
            Assert.AreEqual(11, _readConfig.GetInt32("value"));
        }

        [Test]
        public void ReadInt64()
        {
            _config["test"] = "test";
            _config["value"] = "11";
            Assert.AreEqual(null, _readConfig.GetInt64("null"));
            Assert.AreEqual(null, _readConfig.GetInt64("space"));
            Assert.AreEqual(null, _readConfig.GetInt64("empty"));
            Assert.Throws<FormatException>(() => _readConfig.GetInt64("test"));
            Assert.AreEqual(11, _readConfig.GetInt64("value"));
        }

        private enum TestEnum
        {
            TestValue = 12
        }

        [Test]
        public void ReadEnum()
        {
            _config["test"] = "test";
            _config["value"] = "12";
            _config["value2"] = "TestValue";
            Assert.AreEqual(null, _readConfig.GetEnum<TestEnum>("null"));
            Assert.AreEqual(null, _readConfig.GetEnum<TestEnum>("space"));
            Assert.AreEqual(null, _readConfig.GetEnum<TestEnum>("empty"));
            Assert.Throws<ArgumentException>(() => _readConfig.GetEnum<TestEnum>("test"));
            Assert.AreEqual(TestEnum.TestValue, _readConfig.GetEnum<TestEnum>("value"));
            Assert.AreEqual(TestEnum.TestValue, _readConfig.GetEnum<TestEnum>("value2"));
        }

        [Test]
        public void ReadTimeSpan()
        {
            _config["test"] = "test";
            _config["value"] = TimeSpan.FromHours(1.5).ToString("g");
            _config["value2"] = TimeSpan.FromDays(365).ToString("c");
            Assert.AreEqual(null, _readConfig.GetTimeSpan("null"));
            Assert.AreEqual(null, _readConfig.GetTimeSpan("space"));
            Assert.AreEqual(null, _readConfig.GetTimeSpan("empty"));
            Assert.Throws<FormatException>(() => _readConfig.GetTimeSpan("test"));
            Assert.AreEqual(TimeSpan.FromHours(1.5), _readConfig.GetTimeSpan("value"));
            Assert.AreEqual(TimeSpan.FromDays(365), _readConfig.GetTimeSpan("value2"));
        }

        [Test]
        public void ReadBool()
        {
            _config["test"] = "test";
            _config["value"] = "true";
            _config["value2"] = "false";
            Assert.AreEqual(null, _readConfig.GetBool("null"));
            Assert.AreEqual(null, _readConfig.GetBool("space"));
            Assert.AreEqual(null, _readConfig.GetBool("empty"));
            Assert.Throws<FormatException>(() => _readConfig.GetBool("test"));
            Assert.AreEqual(true, _readConfig.GetBool("value"));
            Assert.AreEqual(false, _readConfig.GetBool("value2"));
            _config["value3"] = "t";
            _config["value4"] = "f";
            Assert.AreEqual(true, _readConfig.GetBool("value3"));
            Assert.AreEqual(false, _readConfig.GetBool("value4"));
            _config["value5"] = "1";
            _config["value6"] = "0";
            Assert.AreEqual(true, _readConfig.GetBool("value5"));
            Assert.AreEqual(false, _readConfig.GetBool("value6"));
        }
    }
}