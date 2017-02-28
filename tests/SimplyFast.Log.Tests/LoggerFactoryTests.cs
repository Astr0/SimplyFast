using Xunit;

namespace SimplyFast.Log.Tests
{
    public class LoggerFactoryTests
    {
        [Fact]
        public void CanGetRoot()
        {
            var f1 = new LoggerFactory();
            var f1Root = f1.Root;
            Assert.NotNull(f1Root);
            Assert.Equal(f1Root, f1.Root);
            var f2 = new LoggerFactory(rootName: "TestRoot");
            var f2Root = f2.Root;
            Assert.Equal("TestRoot", f2Root.ToString());
            Assert.Equal(f2Root, f2.Root);
            Assert.NotEqual(f1Root, f2Root);
        }

        private class TestOutput : IOutput
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
            var o = new TestOutput();
            var f1 = new LoggerFactory();
            var root = f1.Root;
            root.Severity = Severity.Debug;
            root.Log(Severity.Debug, "test");

            root.Outputs.Add(o);

            root.Log(Severity.Debug, "test");
            Assert.Equal("test", o.LastMessage.Get(LogTokenEx.Names.Message).ToString(null));
            Assert.Equal(Severity.Debug, o.LastMessage.Severity);

            root.Severity = Severity.Error;

            root.Log(Severity.Info, "test1");
            // this should not be logged
            Assert.Equal("test", o.LastMessage.Get(LogTokenEx.Names.Message).ToString(null));
            Assert.Equal(Severity.Debug, o.LastMessage.Severity);

            root.Log(Severity.Error, "error");
            // but this should
            Assert.Equal("error", o.LastMessage.Get(LogTokenEx.Names.Message).ToString(null));
            Assert.Equal(Severity.Error, o.LastMessage.Severity);

            root.Log(Severity.Fatal, "fatal");
            // and this should
            Assert.Equal("fatal", o.LastMessage.Get(LogTokenEx.Names.Message).ToString(null));
            Assert.Equal(Severity.Fatal, o.LastMessage.Severity);
        }

        [Fact]
        public void ChildSeverityOk()
        {
            var f = new LoggerFactory();
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
            f.Dispose();
            Assert.NotEqual(test, f.Get("test"));
        }

        [Fact]
        public void ChildLogToRootAndSelf()
        {
            var f = new LoggerFactory();
            var root = f.Root;
            root.Severity = Severity.Debug;
            var child = f.Get("test");
            var oroot = new TestOutput();
            root.Outputs.Add(oroot);
            child.Log(Severity.Debug);
            Assert.Equal(Severity.Debug, oroot.LastMessage.Severity);
            var ochild = new TestOutput();
            child.Outputs.Add(ochild);
            child.Log(Severity.Warn);
            Assert.Equal(Severity.Warn, oroot.LastMessage.Severity);
            Assert.Equal(Severity.Warn, ochild.LastMessage.Severity);
            f.Get("test2").Log(Severity.Info);
            Assert.Equal(Severity.Info, oroot.LastMessage.Severity);
            Assert.Equal(Severity.Warn, ochild.LastMessage.Severity);
        }

        [Fact]
        public void LogUsesMessageFactory()
        {
            var f = new LoggerFactory(s => MessageEx.Default(Severity.Fatal));
            var root = f.Root;
            root.Severity = Severity.Debug;
            var oroot = new TestOutput();
            root.Outputs.Add(oroot);
            root.Log(Severity.Warn);
            var lastMessage = oroot.LastMessage;
            Assert.Equal(Severity.Fatal, lastMessage.Severity);
            f.Get("test").Log(Severity.Info);
            Assert.Equal(Severity.Fatal, oroot.LastMessage.Severity);
            Assert.NotEqual(lastMessage, oroot.LastMessage);
        }
    }
}