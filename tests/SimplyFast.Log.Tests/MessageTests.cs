using System.Diagnostics.CodeAnalysis;
using SimplyFast.Strings.Tokens;
using Xunit;

namespace SimplyFast.Log.Tests
{
    public class MessageTests
    {
        protected virtual IMessage CreateMessage(Severity severity)
        {
            return MessageEx.Default(severity);
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

        private void MsgSeverityOk(Severity severity)
        {
            var msg = CreateMessage(severity);
            Assert.Equal(severity, msg.Severity);
            Assert.Equal(severity.ToString(), msg.Get(LogTokenEx.Names.Severity).ToString(null));
        }

        [Fact]
        public void CanUseTokens()
        {
            var msg = CreateMessage(Severity.Debug);
            msg.Add(LogTokenEx.Message("test"));
            Assert.Equal("test", msg.Get(LogTokenEx.Names.Message).ToString(null));
            msg.Upsert(LogTokenEx.Message("o.O"));
            Assert.Equal("o.O", msg.Get(LogTokenEx.Names.Message).ToString(null));
            Assert.Null(msg.Get("test"));
            msg.Add(StringTokenEx.FormatString("test", "format {0}", "test"));
            Assert.Equal("format test", msg.Get("test").ToString(null));
        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SequentialMessageTest : MessageTests
    {
        protected override IMessage CreateMessage(Severity severity)
        {
            return MessageEx.Default(severity, StringTokenProviderEx.Sequential());
        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class IndexedMessageTest : MessageTests
    {
        protected override IMessage CreateMessage(Severity severity)
        {
            return MessageEx.Default(severity, StringTokenProviderEx.Indexed());
        }
    }
}