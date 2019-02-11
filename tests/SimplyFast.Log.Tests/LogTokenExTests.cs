using System;
using System.Collections.Generic;
using SimplyFast.Strings.Tokens;
using Xunit;

namespace SimplyFast.Log.Tests
{
    public class LogTokenExTests
    {
        [Fact]
        public void NowIsNow()
        {
            var now = DateTime.Now;
            var token = LogTokenEx.Now();
            Assert.Equal(LogTokenEx.Names.Date, token.Name);
            var parsed = DateTime.ParseExact(token.ToString("O"), "O", null);
            Assert.InRange(parsed, now, now.AddSeconds(1));
        }

        [Fact]
        public void SeverityOk()
        {
            var token = LogTokenEx.Severity(Severity.Error);
            Assert.Equal(LogTokenEx.Names.Severity, token.Name);
            Assert.Equal("ERROR", token.ToString("U"));
            Assert.Equal("ERROR", token.ToString("u"));
            Assert.Equal("error", token.ToString("L"));
            Assert.Equal("error", token.ToString("l"));
            Assert.Equal("Error", token.ToString(null));
            token = LogTokenEx.Severity(Severity.Debug);
            Assert.Equal(LogTokenEx.Names.Severity, token.Name);
            Assert.Equal("DEBUG", token.ToString("U"));
            Assert.Equal("DEBUG", token.ToString("u"));
            Assert.Equal("debug", token.ToString("L"));
            Assert.Equal("debug", token.ToString("l"));
            Assert.Equal("Debug", token.ToString(null));
        }

        private class SomeLogger : ILogger
        {
            private readonly string _name;

            public SomeLogger(string name)
            {
                _name = name;
            }

            public void Log(IMessage message)
            {
            }

            public IOutputs Outputs => null;

            public Severity Severity { get; set; }
            public void Log(Severity severity, IEnumerable<IStringToken> info)
            {
            }

            public override string ToString()
            {
                return _name;
            }

            public void Dispose()
            {
            }
        }

        [Fact]
        public void LoggerOk()
        {
            var token = LogTokenEx.Logger(new SomeLogger("MegaLogger"));
            Assert.Equal(LogTokenEx.Names.Logger, token.Name);
            Assert.Equal("MegaLogger", token.ToString(null));
        }

        [Fact]
        public void MessageOk()
        {
            var token = LogTokenEx.Message("TestMessage");
            Assert.Equal(LogTokenEx.Names.Message, token.Name);
            Assert.Equal("TestMessage", token.ToString(null));
        }

        [Fact]
        public void MessageFormatOk()
        {
            var token = LogTokenEx.Message("{2} {0} {1}", "Message", "is cool", "Test");
            Assert.Equal(LogTokenEx.Names.Message, token.Name);
            Assert.Equal("Test Message is cool", token.ToString(null));
        }
    }
}