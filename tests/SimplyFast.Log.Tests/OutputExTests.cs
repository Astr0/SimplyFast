using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimplyFast.Log.Messages;
using Xunit;

namespace SimplyFast.Log.Tests
{
    public class OutputExTests
    {
        private class SomeOutput : IOutput
        {
            public readonly List<IMessage> Logged = new List<IMessage>();

            public bool Throw;
            public bool Disposed { get; private set; }


            public void Log(IMessage message)
            {
                if (Throw)
                    throw new Exception("test");

                Logged.Add(message);
            }

            public void Dispose()
            {
                Disposed = true;
            }
        }

        [Fact]
        public void DefaultOutputsOk()
        {
            var outputs = OutputEx.DefaultOutputs();
            Assert.Equal(0, outputs.Count);
            Assert.True(outputs.SequenceEqual(Enumerable.Empty<IOutput>()));

            var o1 = new SomeOutput();
            var message = MessageEx.Default(Severity.Debug);
            outputs.Add(o1);
            Assert.Equal(1, outputs.Count);
            Assert.True(outputs.SequenceEqual(new[] { o1 }));

            o1.Throw = true;
            outputs.Log(message);
            Assert.Equal(0, o1.Logged.Count);

            o1.Throw = false;
            outputs.Log(message);
            Assert.True(o1.Logged.SequenceEqual(new[] { message }));

            var o2 = new SomeOutput();
            outputs.Add(o2);
            Assert.Equal(2, outputs.Count);
            Assert.True(new[] { o1, o2 }.All(c => outputs.Contains(c)));
            outputs.Log(message);
            Assert.True(o1.Logged.SequenceEqual(new[] { message, message }));
            Assert.True(o2.Logged.SequenceEqual(new[] { message }));
            o1.Throw = true;
            outputs.Log(message);
            Assert.True(o1.Logged.SequenceEqual(new[] { message, message }));
            Assert.True(o2.Logged.SequenceEqual(new[] { message, message }));
            o1.Throw = false;
            o2.Throw = true;
            outputs.Log(message);
            Assert.True(o1.Logged.SequenceEqual(new[] { message, message, message }));
            Assert.True(o2.Logged.SequenceEqual(new[] { message, message }));

            outputs.Remove(o1);
            Assert.Equal(1, outputs.Count);
            Assert.True(outputs.SequenceEqual(new[] { o2 }));

            o2.Throw = false;
            outputs.Log(message);
            Assert.True(o1.Logged.SequenceEqual(new[] { message, message, message }));
            Assert.True(o2.Logged.SequenceEqual(new[] { message, message, message }));

            outputs.Dispose();
            Assert.False(o1.Disposed);
            Assert.True(o2.Disposed);
        }

        [Fact]
        public void OutputSeverityOk()
        {
            var output = new SomeOutput();
            var errorOutput = output.OutputSeverity(Severity.Error);
            var debugMsg = MessageEx.Default(Severity.Debug);
            var infoMsg = MessageEx.Default(Severity.Info);
            var errorMsg = MessageEx.Default(Severity.Error);
            var fatalMsg = MessageEx.Default(Severity.Fatal);
            var offMsg = MessageEx.Default(Severity.Off);
            errorOutput.Log(debugMsg);
            errorOutput.Log(infoMsg);
            errorOutput.Log(errorMsg);
            errorOutput.Log(fatalMsg);
            errorOutput.Log(offMsg);
            Assert.True(output.Logged.SequenceEqual(new[] { errorMsg, fatalMsg }));
        }

        [Fact]
        public void TextWriterOk()
        {
            using (var sw = new StringWriter())
            {
                var writer = new SomeWriter();
                var output = OutputEx.TextWriter(sw, writer);
                var msg1 = MessageEx.Default(Severity.Debug);
                var msg2 = MessageEx.Default(Severity.Error);
                output.Log(msg1);
                output.Log(msg2);
                Assert.True(writer.Written.All(x => x.Item1 == sw));
                Assert.True(writer.Written.Select(x => x.Item2).SequenceEqual(new[] { msg1, msg2 }));
                Assert.Equal("12", sw.ToString());
            }
        }

        [Fact]
        public void ConsoleOk()
        {
            var writer = new SomeWriter();
            var output = OutputEx.Console(writer);
            var debug = MessageEx.Default(Severity.Debug);
            var info = MessageEx.Default(Severity.Info);
            var error = MessageEx.Default(Severity.Error);
            var fatal = MessageEx.Default(Severity.Fatal);
            output.Log(debug);
            output.Log(info);
            output.Log(error);
            output.Log(fatal);
            Assert.Equal(Console.Out, writer.Written[0].Item1);
            Assert.Equal(Console.Out, writer.Written[1].Item1);
            Assert.Equal(Console.Error, writer.Written[2].Item1);
            Assert.Equal(Console.Error, writer.Written[3].Item1);
            Assert.True(writer.Written.Select(x => x.Item2).SequenceEqual(new[] { debug, info, error, fatal }));
        }

        private static string ReadAllTextTest(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(file))
            {
                return sr.ReadToEnd();
            }
        }


        [Fact]
        public void FilesOk()
        {
            var tmp = Path.GetTempFileName();
            try
            {
                var writer = new SomeWriter();
                var debug = MessageEx.Default(Severity.Debug);
                using (var output = OutputEx.File(tmp, writer, false))
                {
                    output.Log(debug);
                    Assert.True(writer.Written.Select(x => x.Item2).SequenceEqual(new[] {debug}));
                    Assert.Equal("1", ReadAllTextTest(tmp));
                }
                using (var output = OutputEx.File(tmp, writer))
                {
                    output.Log(debug);
                    Assert.True(writer.Written.Select(x => x.Item2).SequenceEqual(new[] { debug, debug }));
                    Assert.Equal("12", ReadAllTextTest(tmp));
                }
                using (var output = OutputEx.File(tmp, writer, false))
                {
                    output.Log(debug);
                    Assert.True(writer.Written.Select(x => x.Item2).SequenceEqual(new[] { debug, debug, debug }));
                    Assert.Equal("3", ReadAllTextTest(tmp));
                }
            }
            finally
            {
                File.Delete(tmp);
            }
        }

        private class SomeWriter : IWriter
        {
            public readonly List<Tuple<TextWriter, IMessage>> Written = new List<Tuple<TextWriter, IMessage>>();

            public void Write(TextWriter writer, IMessage message)
            {
                Written.Add(Tuple.Create(writer, message));
                writer.Write(Written.Count);
            }
        }
    }
}