using System;
using System.Globalization;
using SimplyFast.Strings.Tokens;
using Xunit;

namespace SimplyFast.Tests.Strings.Tokens
{
    public class StringTokenExTests
    {
        [Fact]
        public void TokensComparedInsensitive()
        {
            var comparer = StringTokenEx.NameComparer;
            Assert.True(comparer.Equals(null, null));
            Assert.False(comparer.Equals(null, "test"));
            Assert.False(comparer.Equals("test", null));
            Assert.True(comparer.Equals("test", "test"));
            Assert.False(comparer.Equals("test", "test1"));
            Assert.False(comparer.Equals("test1", "test"));
            Assert.True(comparer.Equals("test", "TEST"));
            Assert.True(comparer.Equals("test", "Test"));
            Assert.True(comparer.Equals("TEST", "Test"));
            Assert.True(comparer.Equals("tESt", "TesT"));
        }

        [Fact]
        public void NameEqualsOk()
        {
            var str = StringTokenEx.String("test", "1");
            Assert.True(str.NameEquals("test"));
            Assert.True(str.NameEquals("TEST"));
            Assert.True(str.NameEquals("Test"));
            Assert.False(str.NameEquals("none"));
            Assert.False(str.NameEquals(""));
            Assert.False(str.NameEquals(null));
        }

        [Fact]
        public void StringOk()
        {
            var t = StringTokenEx.String("test", "t");
            Assert.Equal("test", t.Name);
            Assert.Equal("t", t.ToString(null));
            Assert.Equal("t", t.ToString("f"));
        }

        [Fact]
        public void FormatStringOk()
        {
            var t = StringTokenEx.FormatString("test", "t{0}{1}", "es", "t");
            Assert.Equal("test", t.Name);
            Assert.Equal("test", t.ToString(null));
            Assert.Equal("test", t.ToString("f"));
        }

        private class ToStringDiff
        {
            public int I;
            public override string ToString()
            {
                ++I;
                return I.ToString();
            }
        }

        [Fact]
        public void FormatStringCaches()
        {
            var diff = new ToStringDiff();
            var t = StringTokenEx.FormatString("test", "t{0}{1}", "est", diff);
            Assert.Equal("test", t.Name);
            Assert.Equal("test1", t.ToString(null));
            Assert.Equal("test1", t.ToString("f"));
            Assert.Equal(1, diff.I);
        }

        [Fact]
        public void DateTimeOk()
        {
            var dt = DateTime.Now.AddDays(-1);
            var t = StringTokenEx.DateTime("test", dt);
            Assert.Equal("test", t.Name);
            Assert.Equal(dt.ToString(CultureInfo.CurrentCulture), t.ToString(null));
            Assert.Equal(dt.ToString("t"), t.ToString("t"));
            Assert.Equal(dt.ToString("G"), t.ToString("G"));
            Assert.Equal(dt.ToString("d"), t.ToString("d"));
        }

        [Fact]
        public void FuncOk()
        {
            var t = StringTokenEx.Func("test", f => "tt" + f);
            Assert.Equal("test", t.Name);
            Assert.Equal("tt", t.ToString(null));
            Assert.Equal("ttf", t.ToString("f"));
            Assert.Equal("tto.O", t.ToString("o.O"));
        }
    }
}