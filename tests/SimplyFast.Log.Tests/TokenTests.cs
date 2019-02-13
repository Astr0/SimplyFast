using System;
using SimplyFast.Log.Messages;
using Xunit;

namespace SimplyFast.Log.Tests
{
    public class TokenTests
    {
        [Fact]
        public void NowIsNow()
        {
            var now = DateTime.Now;
            var message = MessageEx.Default(Severity.Fatal);
            var token = message.Get(MessageTokens.Date, "O");
            var parsed = DateTime.ParseExact(token, "O", null);
            Assert.InRange(parsed, now, now.AddSeconds(1));
        }

        [Fact]
        public void SeverityOk()
        {
            var message = MessageEx.Default(Severity.Error);
            Assert.Equal("ERROR", message.Get(MessageTokens.Severity, "U"));
            Assert.Equal("ERROR", message.Get(MessageTokens.Severity, "u"));
            Assert.Equal("error", message.Get(MessageTokens.Severity, "L"));
            Assert.Equal("error", message.Get(MessageTokens.Severity, "l"));
            Assert.Equal("Error", message.Get(MessageTokens.Severity));
            message = MessageEx.Default(Severity.Debug);
            Assert.Equal("DEBUG", message.Get(MessageTokens.Severity, "U"));
            Assert.Equal("DEBUG", message.Get(MessageTokens.Severity, "u"));
            Assert.Equal("debug", message.Get(MessageTokens.Severity, "L"));
            Assert.Equal("debug", message.Get(MessageTokens.Severity, "l"));
            Assert.Equal("Debug", message.Get(MessageTokens.Severity));
        }

        private class SomeLogger : ILogger
        {
            public IMessage LastMessage { get; private set; }

            public SomeLogger(string name)
            {
                Name = name;
            }

            public IMessageFactory MessageFactory { get; } = LoggerEx.CreateDefaultMessageFactory();

            public void Log(IMessage message)
            {
                LastMessage = message;
            }

            public string Name { get; }
            public IOutputs Outputs => null;
            public Severity Severity { get; set; }
            
            public override string ToString()
            {
                return Name;
            }
        }

        [Fact]
        public void LoggerOk()
        {
            var logger = new SomeLogger("MegaLogger");
            var message = MessageEx.Default(Severity.Fatal, logger: logger);
            Assert.Equal("MegaLogger", message.Get(MessageTokens.Logger));
        }

        [Fact]
        public void MessageOk()
        {
            var message = MessageEx.Default(Severity.Fatal, "TestMessage");
            Assert.Equal("TestMessage", message.Get(MessageTokens.Message));
        }

        [Fact]
        public void MessageFormatOk()
        {
            var logger = new SomeLogger("test");
            logger.Log(Severity.Fatal, "{2} {0} {1}", "Message", "is cool", "Test");
            var message = logger.LastMessage;
            Assert.Equal("Test Message is cool", message.Get(MessageTokens.Message));
        }
    }
}