using Xunit;

namespace SimplyFast.Log.Tests
{
    public class TokenStringTests
    {
        [Fact]
        public void TokenStringWorksForNormalMessages()
        {
            var tokenString = WriterEx.TokenString("Test %Severity:U%-%Message%!");
            Assert.Equal("Test DEBUG-MyMessage!", tokenString.ToString(MessageEx.Default(Severity.Debug, "MyMessage")));
            Assert.Equal("Test INFO-%!", tokenString.ToString(MessageEx.Default(Severity.Info, "%")));
        }

        [Fact]
        public void TokenStringWorksForCustomMessages()
        {
            var custom =
                LoggerEx.CustomMessage(Severity.Error, (m, t, f) => t.Token == "TOKEN" ? f == "i" ? "42" : "fmt:" + f : null);
            var first = WriterEx.TokenString("Test %TOKEN:i%");
            Assert.Equal("Test 42", first.ToString(custom));
            var second = WriterEx.TokenString("Test `%SomeToken%` is %TOKEN:m%");
            Assert.Equal("Test `` is fmt:m", second.ToString(custom));
        }

        [Fact]
        public void NoTokensStringOk()
        {
            const string always = "Let the force be with you! Always";
            var tokenString = WriterEx.TokenString(always);
            Assert.Equal(always, tokenString.ToString(MessageEx.Default(Severity.Debug, "MyMessage")));
            Assert.Equal(always, tokenString.ToString(MessageEx.Default(Severity.Info, "%")));
        }

        [Fact]
        public void OnlyMessageWorks()
        {
            var tokenString = WriterEx.TokenString("%Message%");
            Assert.Equal("MyMessage", tokenString.ToString(MessageEx.Default(Severity.Debug, "MyMessage")));
            Assert.Equal("%", tokenString.ToString(MessageEx.Default(Severity.Info, "%")));
        }

        [Fact]
        public void PercentWorks()
        {
            var tokenString = WriterEx.TokenString("100%%%Message%");
            Assert.Equal("100%MyMessage", tokenString.ToString(MessageEx.Default(Severity.Debug, "MyMessage")));
            Assert.Equal("100%Ok", tokenString.ToString(MessageEx.Default(Severity.Info, "Ok")));
        }


    }
}