using SimplyFast.Log.Messages;
using Xunit;

namespace SimplyFast.Log.Tests
{
    public class LoggerFactoryTests
    {
        [Fact]
        public void CanGetRoot()
        {
            var f1 = LoggerEx.CreateDefaultFactory();
            var f1Root = f1.Root;
            Assert.NotNull(f1Root);
            Assert.Equal(f1Root, f1.Root);
            var f2 = LoggerEx.CreateDefaultFactory("TestRoot");
            var f2Root = f2.Root;
            Assert.Equal("TestRoot", f2Root.ToString());
            Assert.Equal(f2Root, f2.Root);
            Assert.NotEqual(f1Root, f2Root);
        }

        private class SomeOutput : IOutput
        {
            public IMessage LastMessage { get; private set; }
            public void Dispose()
            {
            }

            public void Log(IMessage message)
            {
                LastMessage = message;
            }
        }

        [Fact]
        public void RootCanLogMessage()
        {
            var o = new SomeOutput();
            var f1 = LoggerEx.CreateDefaultFactory();
            var root = f1.Root;
            root.Severity = Severity.Debug;
            root.Log(Severity.Debug, "test");

            root.Outputs.Add(o);

            root.Log(Severity.Debug, "test");
            Assert.Equal("test", o.LastMessage.Get(MessageTokens.Message));
            Assert.Equal(Severity.Debug, o.LastMessage.Severity);

            root.Severity = Severity.Error;

            root.Log(Severity.Info, "test1");
            // this should not be logged
            Assert.Equal("test", o.LastMessage.Get(MessageTokens.Message));
            Assert.Equal(Severity.Debug, o.LastMessage.Severity);

            root.Log(Severity.Error, "error");
            // but this should
            Assert.Equal("error", o.LastMessage.Get(MessageTokens.Message));
            Assert.Equal(Severity.Error, o.LastMessage.Severity);

            root.Log(Severity.Fatal, "fatal");
            // and this should
            Assert.Equal("fatal", o.LastMessage.Get(MessageTokens.Message));
            Assert.Equal(Severity.Fatal, o.LastMessage.Severity);
        }

        [Fact]
        public void ChildSeverityOk()
        {
            var f = LoggerEx.CreateDefaultFactory();
            var root = f.Root;
            var test = f.Get("test");
            Assert.Equal(test, f.Get("test"));
            var test2 = f.Get(2);
            Assert.Equal(test2, f.Get(2));
            Assert.Equal("test", test.ToString());
            Assert.Equal("2", test2.ToString());
            root.Severity = Severity.Debug;
            Assert.Equal(Severity.Debug, test.Severity);
            Assert.Equal(Severity.Debug, test2.Severity);
            root.Severity = Severity.Info;
            Assert.Equal(Severity.Info, test.Severity);
            Assert.Equal(Severity.Info, test2.Severity);
            test.Severity = Severity.Debug;
            Assert.Equal(Severity.Debug, test.Severity);
            Assert.Equal(Severity.Info, test2.Severity);
            root.Severity = Severity.Fatal;
            Assert.Equal(Severity.Debug, test.Severity);
            Assert.Equal(Severity.Fatal, test2.Severity);
            //f.Dispose();
            //Assert.NotEqual(test, f.Get("test"));
        }

        [Fact]
        public void ChildLogToRootAndSelf()
        {
            var f = LoggerEx.CreateDefaultFactory();
            var root = f.Root;
            root.Severity = Severity.Debug;
            var child = f.Get("test");
            var rootOutput = new SomeOutput();
            root.Outputs.Add(rootOutput);
            child.Log(Severity.Debug, "d");
            Assert.Equal(Severity.Debug, rootOutput.LastMessage.Severity);
            var childOutput = new SomeOutput();
            child.Outputs.Add(childOutput);
            child.Log(Severity.Warn, "W");
            Assert.Equal(Severity.Warn, rootOutput.LastMessage.Severity);
            Assert.Equal(Severity.Warn, childOutput.LastMessage.Severity);
            f.Get("test2").Log(Severity.Info, "I");
            Assert.Equal(Severity.Info, rootOutput.LastMessage.Severity);
            Assert.Equal(Severity.Warn, childOutput.LastMessage.Severity);
        }

        [Fact]
        public void LogUsesMessageFactory()
        {
            //var f = new LoggerFactory(s => MessageEx.Default(Severity.Fatal));
            var f = LoggerEx.CreateDefaultFactory();
            var root = f.Root;
            root.Severity = Severity.Debug;
            var rootOutput = new SomeOutput();
            root.Outputs.Add(rootOutput);
            root.Log(Severity.Warn, "W");
            var lastMessage = rootOutput.LastMessage;
            Assert.Equal(Severity.Fatal, lastMessage.Severity);
            f.Get("test").Log(Severity.Info, "I");
            Assert.Equal(Severity.Fatal, rootOutput.LastMessage.Severity);
            Assert.NotEqual(lastMessage, rootOutput.LastMessage);
        }
    }
}