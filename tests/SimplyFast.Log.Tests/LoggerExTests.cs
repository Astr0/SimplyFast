using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SimplyFast.Strings.Tokens;
using Xunit;

namespace SimplyFast.Log.Tests
{
    public class LoggerExTests
    {
        private class SomeLogger : ILogger
        {
            public void Dispose()
            {
                
            }

            public void Log(IMessage message)
            {
                throw new Exception("Shouldn't be called");
            }

            public IOutputs Outputs => null;
            public Severity Severity { get; set; }

            private Severity? _lastSeverity;
            private IStringToken[] _lastTokens;

            [SuppressMessage("ReSharper", "UnusedParameter.Local")]
            [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
            public void LastWas(Severity severity, string name, string str)
            {
                Assert.Equal(severity, _lastSeverity);
                if (name != null)
                {
                    var token = _lastTokens.FirstOrDefault(x => x.NameEquals(name));
                    Assert.NotNull(token);
                    Assert.Equal(str, token.ToString(null));
                }
                else
                {
                    Assert.Empty(_lastTokens);
                }
            }

            public void Log(Severity severity, IEnumerable<IStringToken> info)
            {
                _lastSeverity = severity;
                _lastTokens = info.ToArray();
            }
        }

        [Fact]
        public void StaticMessagesWorks()
        {
            var l = new SomeLogger();
            l.Log(Severity.Debug);
            l.LastWas(Severity.Debug, null, null);
            l.Log(Severity.Error, LogTokenEx.Message("test"));
            l.LastWas(Severity.Error, LogTokenEx.Names.Message, "test");
            l.Log(Severity.Fatal, "test1");
            l.LastWas(Severity.Fatal, LogTokenEx.Names.Message, "test1");
            l.Debug("testD");
            l.LastWas(Severity.Debug, LogTokenEx.Names.Message, "testD");
            l.Info("testI");
            l.LastWas(Severity.Info, LogTokenEx.Names.Message, "testI");
            l.Warning("testW");
            l.LastWas(Severity.Warn, LogTokenEx.Names.Message, "testW");
            l.Error("testE");
            l.LastWas(Severity.Error, LogTokenEx.Names.Message, "testE");
            l.Fatal("testF");
            l.LastWas(Severity.Fatal, LogTokenEx.Names.Message, "testF");
        }

        [Fact]
        public void FormatMessagesWorks()
        {
            var l = new SomeLogger();
            l.Log(Severity.Fatal, "test{0}", 1);
            l.LastWas(Severity.Fatal, LogTokenEx.Names.Message, "test1");
            l.Debug("test{0}", "D");
            l.LastWas(Severity.Debug, LogTokenEx.Names.Message, "testD");
            l.Info("test{0}", "I");
            l.LastWas(Severity.Info, LogTokenEx.Names.Message, "testI");
            l.Warning("test{0}", "W");
            l.LastWas(Severity.Warn, LogTokenEx.Names.Message, "testW");
            l.Error("test{0}", "E");
            l.LastWas(Severity.Error, LogTokenEx.Names.Message, "testE");
            l.Fatal("test{0}", "F");
            l.LastWas(Severity.Fatal, LogTokenEx.Names.Message, "testF");
        }
    }
}