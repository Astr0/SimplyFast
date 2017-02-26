using System;
using System.Linq;
using Xunit;
using SimplyFast.Strings;

namespace SimplyFast.Tests.Strings
{
    
    public class StringExTests
    {
        [Fact]
        public void LeftChecksForNull()
        {
            const string s = null;
            Assert.Throws(typeof(ArgumentNullException), () => s.Left(1));
            Assert.Throws(typeof(ArgumentNullException), () => ((string)null).Left(1));
            Assert.Throws(typeof(ArgumentNullException), () => StringEx.Left(null, 20));
        }

        [Fact]
        public void LeftIsAnExtensionMethod()
        {
            Assert.NotNull("test".Left(2));
        }

        [Fact]
        public void LeftWorksIfStringLonger()
        {
            Assert.Equal("te", "test".Left(2));
        }

        [Fact]
        public void LeftWorksIfStringShorter()
        {
            Assert.Equal("test", "test".Left(20));
        }

        [Fact]
        public void QuoteIsAnExtensionMethod()
        {
            Assert.NotNull("test".Quote());
            Assert.NotNull("test".Quote('?'));
        }

        [Fact]
        public void QuoteThrowsOnNull()
        {
            const string s = null;
            Assert.Throws<ArgumentNullException>(() => s.Quote());
            Assert.Throws<ArgumentNullException>(() => s.Quote('r'));
        }

        [Fact]
        public void QuoteWorksForStringEx()
        {
            Assert.Equal("\"test\"", "test".Quote());
            Assert.Equal("?test?", "test".Quote('?'));
        }

        [Fact]
        public void QuoteWorksForStringsWithQuotes()
        {
            Assert.Equal("\"te\"\"st te\"\"\"\"st\"", "te\"st te\"\"st".Quote());
            Assert.Equal("?t\"e??st te????st?", "t\"e?st te??st".Quote('?'));
        }

        [Fact]
        public void RepeatWorksIfCountLessThanZero()
        {
            Assert.Equal(string.Empty, StringEx.Repeat("test", -50));
            Assert.Equal(string.Empty, StringEx.Repeat("test", 0));
        }

        [Fact]
        public void RepeatWorksWithMultipleChars()
        {
            Assert.Equal("AbAbAbAbAb", StringEx.Repeat("Ab", 5));
        }

        [Fact]
        public void RepeatWorksWithNullOrEmpty()
        {
            Assert.Equal(string.Empty, StringEx.Repeat(string.Empty, 50));
            Assert.Equal(string.Empty, StringEx.Repeat(null, 50000));
        }

        [Fact]
        public void RepeatWorksWithSingleChar()
        {
            Assert.Equal("AAAA", StringEx.Repeat("A", 4));
        }

        [Fact]
        public void RightChecksForNull()
        {
            Assert.Throws(typeof(ArgumentNullException), () => StringEx.Right(null, 20));
        }

        [Fact]
        public void RightIsAnExtensionMethod()
        {
            Assert.NotNull("test".Right(2));
        }

        [Fact]
        public void RightWorksIfStringLonger()
        {
            Assert.Equal("st", "test".Right(2));
        }

        [Fact]
        public void RightWorksIfStringShorter()
        {
            Assert.Equal("test", "test".Right(20));
        }

        [Fact]
        public void SplitQuotedWorksForDoubleQuoted()
        {
            const string s = "12,\"\"\"Just,Quoted\"\"\"";
            var split = s.SplitQuoted();
            Assert.Equal(2, split.Length);
            Assert.Equal("12", split[0]);
            Assert.Equal("\"Just,Quoted\"", split[1]);
        }

        [Fact]
        public void SplitQuotedWorksForEdgeQuoted()
        {
            const string s = "mid\"dle,normal,\"\"\"Double\"\"\",End\",\"Test,\"\"Quoted\"\"\",\"forgot,some,quote";
            var split = s.SplitQuoted();
            Assert.Equal(6, split.Length);
            Assert.Equal("mid\"dle", split[0]);
            Assert.Equal("normal", split[1]);
            Assert.Equal("\"Double\"", split[2]);
            Assert.Equal("End\"", split[3]);
            Assert.Equal("Test,\"Quoted\"", split[4]);
            Assert.Equal("forgot,some,quote", split[5]);
        }

        [Fact]
        public void SplitQuotedWorksForEmptyValues()
        {
            const string s = "1,";
            var splitQuoted = s.SplitQuoted();
            var split = s.Split(',');
            Assert.True(split.SequenceEqual(splitQuoted));
            const string s1 = ",1";
            splitQuoted = s1.SplitQuoted();
            split = s1.Split(',');
            Assert.True(split.SequenceEqual(splitQuoted));
            const string s2 = "1,,2";
            splitQuoted = s2.SplitQuoted();
            split = s2.Split(',');
            Assert.True(split.SequenceEqual(splitQuoted));
            const string s3 = ",,";
            splitQuoted = s3.SplitQuoted();
            split = s3.Split(',');
            Assert.True(split.SequenceEqual(splitQuoted));
        }

        [Fact]
        public void SplitQuotedWorksForMalformed()
        {
            // Excel 2007 - like parsing
            const string s = "this,i\"s,\"\"not\",my,\"\"problem\"\",\"\"amigo\"\"really\"\",:)";
            var split = s.SplitQuoted();
            Assert.Equal(7, split.Length);
            Assert.Equal("this", split[0]);
            Assert.Equal("i\"s", split[1]);
            Assert.Equal("not\"", split[2]);
            Assert.Equal("my", split[3]);
            Assert.Equal("problem\"\"", split[4]);
            Assert.Equal("amigo\"\"really\"\"", split[5]);
            Assert.Equal(":)", split[6]);
        }

        [Fact]
        public void SplitQuotedWorksForNonQuoted()
        {
            const string s = "12,tsfd,gfdgdf,fsfa,xzv,123,sdf.54,54.23";
            var splitted = s.Split(',');
            var quoteSplitted = s.SplitQuoted();
            Assert.True(splitted.SequenceEqual(quoteSplitted));
        }

        [Fact]
        public void SplitQuotedWorksForNormallyQuoted()
        {
            const string s = "1,\"test\",\"Test,\"\"Quoted\"\"\",test,\"\"\"Just,Quoted\"\"\"";
            var split = s.SplitQuoted();
            Assert.Equal(5, split.Length);
            Assert.Equal("1", split[0]);
            Assert.Equal("test", split[1]);
            Assert.Equal("Test,\"Quoted\"", split[2]);
            Assert.Equal("test", split[3]);
            Assert.Equal("\"Just,Quoted\"", split[4]);
        }

        [Fact]
        public void SplitQuotedWorksForNullOrEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => StringEx.SplitQuoted(null));
            Assert.Equal(0, string.Empty.SplitQuoted().Length);
        }

        [Fact]
        public void SplitQuotedWorksForShortValues()
        {
            const string s = "1,2,3,4";
            var split = s.SplitQuoted();
            Assert.Equal(4, split.Length);
            Assert.Equal("1", split[0]);
            Assert.Equal("2", split[1]);
            Assert.Equal("3", split[2]);
            Assert.Equal("4", split[3]);
        }

        [Fact]
        public void SubstringSafeIsAnExtensionMethod()
        {
            Assert.NotNull("test".SubstringSafe(1));
            Assert.NotNull("test".SubstringSafe(1, 2));
        }

        [Fact]
        public void SubstringSafeReturnsEmptyForNull()
        {
            Assert.Equal(string.Empty, StringEx.SubstringSafe(null, 20));
        }

        [Fact]
        public void SubstringSafeWorksIfInvalidParams()
        {
            Assert.Equal("test", "test".SubstringSafe(-10));
            Assert.Equal("te", "test".SubstringSafe(-10, 2));
            Assert.Equal(string.Empty, "test".SubstringSafe(1, -1));
            Assert.Equal(string.Empty, "test".SubstringSafe(-5, -6));
        }

        [Fact]
        public void SubstringSafeWorksIfStringLonger()
        {
            Assert.Equal("est", "test".SubstringSafe(1));
            Assert.Equal("es", "test".SubstringSafe(1, 2));
        }

        [Fact]
        public void SubstringSafeWorksIfStringShorter()
        {
            Assert.Equal("test", "test".SubstringSafe(0, 10));
            Assert.Equal(string.Empty, "test".SubstringSafe(5));
            Assert.Equal(string.Empty, "test".SubstringSafe(5, 6));
        }

        [Fact]
        public void SkipWorks()
        {
            Assert.Equal("ab", "12ab".Skip("12"));
            Assert.Equal("12ab", "12ab".Skip("TT"));
        }

        [Fact]
        public void RemoveAllWorks()
        {
            Assert.Equal("ZeroCool", "`Zero`[Cool]".RemoveAll('`', '[', ']'));
            Assert.Equal("ZeroCool", "ZeroCool".RemoveAll('`', '[', ']'));
            Assert.Equal("ZeroCool", "ZeroCool".RemoveAll());
        }
    }
}