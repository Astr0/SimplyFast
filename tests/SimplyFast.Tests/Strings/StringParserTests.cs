using NUnit.Framework;
using SF.Strings;

namespace SF.Tests.Strings
{
    [TestFixture]
    public class StringParserTests
    {
        [SetUp]
        public void SetUp()
        {
            _parser = new StringParser(TestString);
        }

        private const string TestString = "ab1ab2abab3ababab4ab1ab2abab3ababab4";
        private StringParser _parser;

        public void FullTests()
        {
            BaseTests();
            _parser.Reset();
            MoveTests();
            _parser.Reset();
            SkipTests();
            _parser.Reset();
            BackTests();
        }

        [Test]
        public void BackTests()
        {
            _parser.Index = _parser.Length;
            Assert.AreEqual(TestString.Length - 1, _parser.BackTo("4").Index);
            Assert.AreEqual(TestString.Length - 1, _parser.BackToEndOf("ab").Index);
            Assert.AreEqual("abab", _parser.BackToEndOf("ab", 3).SubstringTo("4"));
            Assert.AreEqual("ababab", _parser.BackTo("ab", 3).SubstringTo("4"));
            Assert.AreEqual("ababab", _parser.BackToEndOf("3").SubstringTo("4"));
        }

        [Test]
        public void BaseTests()
        {
            Assert.AreEqual(TestString, _parser.Text);
            Assert.AreEqual(TestString.Length, _parser.Length);
            Assert.AreEqual("ab", _parser.SubstringTo("1"));
            Assert.AreEqual("1ab2abab3", _parser.SubstringTo("ababab4"));
            Assert.AreEqual("ab1ab2abab3", _parser.Left);
            Assert.AreEqual("ababab4ab1ab2abab3ababab4", _parser.Right);
        }

        [Test]
        public void CloneTests()
        {
            BaseTests();
            var clone = _parser.Clone();
            Assert.AreEqual(_parser.Text, clone.Text);
            Assert.AreEqual(_parser.Length, clone.Length);
            Assert.AreEqual(_parser.Index, clone.Index);
            _parser = clone;
            _parser.Reset();
            FullTests();
        }

        [Test]
        public void MoveTests()
        {
            Assert.AreEqual(0, _parser.Index);
            Assert.IsTrue(_parser.Start);
            Assert.IsFalse(_parser.End);
            _parser.Skip(1);
            Assert.AreEqual(1, _parser.Index);
            Assert.IsFalse(_parser.Start);
            Assert.IsFalse(_parser.End);
            _parser.Skip(1000);
            Assert.AreEqual(TestString.Length, _parser.Index);
            Assert.IsFalse(_parser.Start);
            Assert.IsTrue(_parser.End);
            _parser.Back(1);
            Assert.AreEqual(TestString.Length - 1, _parser.Index);
            Assert.IsFalse(_parser.Start);
            Assert.IsFalse(_parser.End);
            _parser.Back(1000);
            Assert.IsTrue(_parser.Start);
            Assert.IsFalse(_parser.End);
            Assert.AreEqual(0, _parser.Index);
            _parser.Index = TestString.Length / 2;
            Assert.IsFalse(_parser.Start);
            Assert.IsFalse(_parser.End);
            Assert.AreEqual(TestString.Length / 2, _parser.Index);
        }

        [Test]
        public void ResetTests()
        {
            BaseTests();
            _parser.Reset();
            FullTests();
        }


        [Test]
        public void SkipTests()
        {
            Assert.AreEqual("1ab", _parser.SkipTo("1").SubstringTo("2"));
            Assert.AreEqual("ab", _parser.SkipToEndOf("ab").SubstringTo("3"));
            Assert.AreEqual("ab", _parser.SkipTo("ab", 3).SubstringTo("4"));
            Assert.AreEqual("3ababab", _parser.SkipToEndOf("ab", 4).SubstringTo("4"));
        }
    }
}