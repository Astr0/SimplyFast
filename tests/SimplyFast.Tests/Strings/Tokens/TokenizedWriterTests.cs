using System;
using SimplyFast.Strings.Tokens;
using Xunit;

namespace SimplyFast.Tests.Strings.Tokens
{
    public class TokenizedWriterTests
    {
        private static readonly DateTime _dt = DateTime.Now.AddDays(-1);
        private readonly IStringTokenStorage _provider;

        public TokenizedWriterTests()
        {
            var s = StringTokenProviderEx.Sequential();
            s.Add(StringTokenEx.String("test", "1"));
            s.Add(StringTokenEx.DateTime("dt", _dt));
            _provider = s;
        }

        [Fact]
        public void WriteConstStringsOk()
        {
            var tp = new TokenizedWriter();
            Assert.Equal("", tp.GetString(x => null));
            Assert.Equal("", tp.GetString(_provider));
            tp.Add("String");
            Assert.Equal("String", tp.GetString(x => null));
            Assert.Equal("String", tp.GetString(_provider));
            tp.Add("Works");
            Assert.Equal("StringWorks", tp.GetString(x => null));
            Assert.Equal("StringWorks", tp.GetString(_provider));
        }

        [Fact]
        public void WriteTokenStringsOk()
        {
            var tp = new TokenizedWriter
            {
                "String",
                {"test", null },
                {"dt", "g" }
            };
            var dtString = _dt.ToString("g");
            Assert.Equal("String1" + dtString, tp.GetString(_provider.Get));
            Assert.Equal("String1" + dtString, tp.GetString(_provider));
            Assert.Equal("String%test%%dt:g%", tp.GetString(x => null));
        }

        [Fact]
        public void ParseOk()
        {
            var tp = new TokenizedWriter
            {
                "Str"
            };
            tp.AddTokenizedString("ing%test%%dt:g%!");
            var dtString = _dt.ToString("g");
            Assert.Equal("String1" + dtString + "!", tp.GetString(_provider.Get));
            Assert.Equal("String1" + dtString + "!", tp.GetString(_provider));
            Assert.Equal("String%test%%dt:g%!", tp.GetString(x => null));
        }
    }
}