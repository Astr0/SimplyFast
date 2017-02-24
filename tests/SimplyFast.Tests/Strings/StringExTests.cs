using System;
using System.Linq;
using NUnit.Framework;
using SimplyFast.Strings;

namespace SimplyFast.Tests.Strings
{
    [TestFixture]
    public class StringExTests
    {
        [Test]
        public void LeftChecksForNull()
        {
            const string s = null;
            Assert.Throws(typeof(ArgumentNullException), () => s.Left(1));
            Assert.Throws(typeof(ArgumentNullException), () => ((string)null).Left(1));
            Assert.Throws(typeof(ArgumentNullException), () => StringEx.Left(null, 20));
        }

        [Test]
        public void LeftIsAnExtensionMethod()
        {
            Assert.IsNotNull("test".Left(2));
        }

        [Test]
        public void LeftWorksIfStringLonger()
        {
            Assert.AreEqual("te", "test".Left(2));
        }

        [Test]
        public void LeftWorksIfStringShorter()
        {
            Assert.AreEqual("test", "test".Left(20));
        }

        [Test]
        public void QuoteIsAnExtensionMethod()
        {
            Assert.IsNotNull("test".Quote());
            Assert.IsNotNull("test".Quote('?'));
        }

        [Test]
        public void QuoteThrowsOnNull()
        {
            const string s = null;
            Assert.Throws<ArgumentNullException>(() => s.Quote());
            Assert.Throws<ArgumentNullException>(() => s.Quote('r'));
        }

        [Test]
        public void QuoteWorksForStringEx()
        {
            Assert.AreEqual("\"test\"", "test".Quote());
            Assert.AreEqual("?test?", "test".Quote('?'));
        }

        [Test]
        public void QuoteWorksForStringsWithQuotes()
        {
            Assert.AreEqual("\"te\"\"st te\"\"\"\"st\"", "te\"st te\"\"st".Quote());
            Assert.AreEqual("?t\"e??st te????st?", "t\"e?st te??st".Quote('?'));
        }

        [Test]
        public void RepeatWorksIfCountLessThanZero()
        {
            Assert.AreEqual(string.Empty, StringEx.Repeat("test", -50));
            Assert.AreEqual(string.Empty, StringEx.Repeat("test", 0));
        }

        [Test]
        public void RepeatWorksWithMultipleChars()
        {
            Assert.AreEqual("AbAbAbAbAb", StringEx.Repeat("Ab", 5));
        }

        [Test]
        public void RepeatWorksWithNullOrEmpty()
        {
            Assert.AreEqual(string.Empty, StringEx.Repeat(string.Empty, 50));
            Assert.AreEqual(string.Empty, StringEx.Repeat(null, 50000));
        }

        [Test]
        public void RepeatWorksWithSingleChar()
        {
            Assert.AreEqual("AAAA", StringEx.Repeat("A", 4));
        }

        [Test]
        public void RightChecksForNull()
        {
            Assert.Throws(typeof(ArgumentNullException), () => StringEx.Right(null, 20));
        }

        [Test]
        public void RightIsAnExtensionMethod()
        {
            Assert.IsNotNull("test".Right(2));
        }

        [Test]
        public void RightWorksIfStringLonger()
        {
            Assert.AreEqual("st", "test".Right(2));
        }

        [Test]
        public void RightWorksIfStringShorter()
        {
            Assert.AreEqual("test", "test".Right(20));
        }

        [Test]
        public void SplitQuotedWorksForDoubleQuoted()
        {
            const string s = "12,\"\"\"Just,Quoted\"\"\"";
            var split = s.SplitQuoted();
            Assert.AreEqual(2, split.Length);
            Assert.AreEqual("12", split[0]);
            Assert.AreEqual("\"Just,Quoted\"", split[1]);
        }

        [Test]
        public void SplitQuotedWorksForEdgeQuoted()
        {
            const string s = "mid\"dle,normal,\"\"\"Double\"\"\",End\",\"Test,\"\"Quoted\"\"\",\"forgot,some,quote";
            var split = s.SplitQuoted();
            Assert.AreEqual(6, split.Length);
            Assert.AreEqual("mid\"dle", split[0]);
            Assert.AreEqual("normal", split[1]);
            Assert.AreEqual("\"Double\"", split[2]);
            Assert.AreEqual("End\"", split[3]);
            Assert.AreEqual("Test,\"Quoted\"", split[4]);
            Assert.AreEqual("forgot,some,quote", split[5]);
        }

        [Test]
        public void SplitQuotedWorksForEmptyValues()
        {
            const string s = "1,";
            var splitQuoted = s.SplitQuoted();
            var split = s.Split(',');
            Assert.IsTrue(split.SequenceEqual(splitQuoted));
            const string s1 = ",1";
            splitQuoted = s1.SplitQuoted();
            split = s1.Split(',');
            Assert.IsTrue(split.SequenceEqual(splitQuoted));
            const string s2 = "1,,2";
            splitQuoted = s2.SplitQuoted();
            split = s2.Split(',');
            Assert.IsTrue(split.SequenceEqual(splitQuoted));
            const string s3 = ",,";
            splitQuoted = s3.SplitQuoted();
            split = s3.Split(',');
            Assert.IsTrue(split.SequenceEqual(splitQuoted));
        }

        [Test]
        public void SplitQuotedWorksForMalformed()
        {
            // Excel 2007 - like parsing
            const string s = "this,i\"s,\"\"not\",my,\"\"problem\"\",\"\"amigo\"\"really\"\",:)";
            var split = s.SplitQuoted();
            Assert.AreEqual(7, split.Length);
            Assert.AreEqual("this", split[0]);
            Assert.AreEqual("i\"s", split[1]);
            Assert.AreEqual("not\"", split[2]);
            Assert.AreEqual("my", split[3]);
            Assert.AreEqual("problem\"\"", split[4]);
            Assert.AreEqual("amigo\"\"really\"\"", split[5]);
            Assert.AreEqual(":)", split[6]);
        }

        [Test]
        public void SplitQuotedWorksForNonQuoted()
        {
            const string s = "12,tsfd,gfdgdf,fsfa,xzv,123,sdf.54,54.23";
            var splitted = s.Split(',');
            var quoteSplitted = s.SplitQuoted();
            Assert.IsTrue(splitted.SequenceEqual(quoteSplitted));
        }

        [Test]
        public void SplitQuotedWorksForNormallyQuoted()
        {
            const string s = "1,\"test\",\"Test,\"\"Quoted\"\"\",test,\"\"\"Just,Quoted\"\"\"";
            var split = s.SplitQuoted();
            Assert.AreEqual(5, split.Length);
            Assert.AreEqual("1", split[0]);
            Assert.AreEqual("test", split[1]);
            Assert.AreEqual("Test,\"Quoted\"", split[2]);
            Assert.AreEqual("test", split[3]);
            Assert.AreEqual("\"Just,Quoted\"", split[4]);
        }

        [Test]
        public void SplitQuotedWorksForNullOrEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => StringEx.SplitQuoted(null));
            Assert.AreEqual(0, string.Empty.SplitQuoted().Length);
        }

        [Test]
        public void SplitQuotedWorksForShortValues()
        {
            const string s = "1,2,3,4";
            var split = s.SplitQuoted();
            Assert.AreEqual(4, split.Length);
            Assert.AreEqual("1", split[0]);
            Assert.AreEqual("2", split[1]);
            Assert.AreEqual("3", split[2]);
            Assert.AreEqual("4", split[3]);
        }

        [Test]
        public void SubstringSafeIsAnExtensionMethod()
        {
            Assert.IsNotNull("test".SubstringSafe(1));
            Assert.IsNotNull("test".SubstringSafe(1, 2));
        }

        [Test]
        public void SubstringSafeReturnsEmptyForNull()
        {
            Assert.AreEqual(string.Empty, StringEx.SubstringSafe(null, 20));
        }

        [Test]
        public void SubstringSafeWorksIfInvalidParams()
        {
            Assert.AreEqual("test", "test".SubstringSafe(-10));
            Assert.AreEqual("te", "test".SubstringSafe(-10, 2));
            Assert.AreEqual(string.Empty, "test".SubstringSafe(1, -1));
            Assert.AreEqual(string.Empty, "test".SubstringSafe(-5, -6));
        }

        [Test]
        public void SubstringSafeWorksIfStringLonger()
        {
            Assert.AreEqual("est", "test".SubstringSafe(1));
            Assert.AreEqual("es", "test".SubstringSafe(1, 2));
        }

        [Test]
        public void SubstringSafeWorksIfStringShorter()
        {
            Assert.AreEqual("test", "test".SubstringSafe(0, 10));
            Assert.AreEqual(string.Empty, "test".SubstringSafe(5));
            Assert.AreEqual(string.Empty, "test".SubstringSafe(5, 6));
        }

        [Test]
        public void SkipWorks()
        {
            Assert.AreEqual("ab", "12ab".Skip("12"));
            Assert.AreEqual("12ab", "12ab".Skip("TT"));
        }

        [Test]
        public void RemoveAllWorks()
        {
            Assert.AreEqual("ZeroCool", "`Zero`[Cool]".RemoveAll('`', '[', ']'));
            Assert.AreEqual("ZeroCool", "ZeroCool".RemoveAll('`', '[', ']'));
            Assert.AreEqual("ZeroCool", "ZeroCool".RemoveAll());
        }
    }
}