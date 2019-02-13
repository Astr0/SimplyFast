using SimplyFast.Log.Messages;
using Xunit;

namespace SimplyFast.Log.Tests
{
    public class MessageTests
    {
        private static IMessage CreateMessage(Severity severity, string msg = null)
        {
            return MessageEx.Default(severity, msg);
        }

        private void MsgSeverityOk(Severity severity)
        {
            var msg = CreateMessage(severity);
            Assert.Equal(severity, msg.Severity);
            Assert.Equal(severity.ToString(), msg.Get(MessageTokens.Severity));
        }

        [Fact]
        public void CanUseTokens()
        {
            var msg = CreateMessage(Severity.Debug, "test");
            Assert.Equal("test", msg.Get(MessageTokens.Message));
        }

        [Fact]
        public void MessageSeverityOk()
        {
            MsgSeverityOk(Severity.Debug);
            MsgSeverityOk(Severity.Info);
            MsgSeverityOk(Severity.Warn);
            MsgSeverityOk(Severity.Error);
            MsgSeverityOk(Severity.Fatal);
            MsgSeverityOk(Severity.Off);
        }
    }
}