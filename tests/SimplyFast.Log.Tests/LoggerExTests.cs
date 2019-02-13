using System.Diagnostics.CodeAnalysis;
using SimplyFast.Log.Messages;
using Xunit;

namespace SimplyFast.Log.Tests
{
    public class LoggerExTests
    {
        private class SomeLogger : ILogger
        {
            public IMessageFactory MessageFactory { get; } = LoggerEx.CreateDefaultMessageFactory();

            public void Log(IMessage message)
            {
                _lastMessage = message;
            }

            public string Name => "Some Logger";
            public IOutputs Outputs => null;
            public Severity Severity { get; set; }

            private IMessage _lastMessage;

            [SuppressMessage("ReSharper", "UnusedParameter.Local")]
            [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
            public void LastWas(Severity severity, string message)
            {
                Assert.Equal(severity, _lastMessage?.Severity);
                Assert.Equal(message, _lastMessage?.Get(MessageTokens.Message));
            }
        }

        [Fact]
        public void StaticMessagesWorks()
        {
            var l = new SomeLogger();
            l.Log(Severity.Debug, "D");
            l.LastWas(Severity.Debug, "D");
            l.Log(Severity.Error, "test");
            l.LastWas(Severity.Error, "test");
            l.Log(Severity.Fatal, "test1");
            l.LastWas(Severity.Fatal, "test1");
            l.Debug("testD");
            l.LastWas(Severity.Debug, "testD");
            l.Info("testI");
            l.LastWas(Severity.Info, "testI");
            l.Warning("testW");
            l.LastWas(Severity.Warn, "testW");
            l.Error("testE");
            l.LastWas(Severity.Error, "testE");
            l.Fatal("testF");
            l.LastWas(Severity.Fatal, "testF");
        }

        [Fact]
        public void FormatMessagesWorks()
        {
            var l = new SomeLogger();
            l.Log(Severity.Fatal, "test{0}", 1);
            l.LastWas(Severity.Fatal, "test1");
            l.Debug("test{0}", "D");
            l.LastWas(Severity.Debug, "testD");
            l.Info("test{0}", "I");
            l.LastWas(Severity.Info, "testI");
            l.Warning("test{0}", "W");
            l.LastWas(Severity.Warn, "testW");
            l.Error("test{0}", "E");
            l.LastWas(Severity.Error, "testE");
            l.Fatal("test{0}", "F");
            l.LastWas(Severity.Fatal, "testF");
        }
    }
}