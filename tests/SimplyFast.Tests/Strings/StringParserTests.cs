using NUnit.Framework;
using SimplyFast.Strings;

namespace SimplyFast.Tests.Strings
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

        private void FullTests()
        {
            BaseTests();
            _parser.Reset();
            MoveTests();
            _parser.Reset();
            SkipTests();
            _parser.Reset();
            BackTests();
            _parser.Reset();
            TrimTests();
            _parser.Reset();
            NextIsTests();
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
        public void TrimTests()
        {
            var src = _parser.Clone();
            _parser = _parser.TrimTo("2");
            Assert.AreEqual("ab1ab", _parser.View);
            Assert.AreEqual(5, _parser.Length);
            Assert.AreEqual(0, _parser.Index);
            Assert.AreEqual(TestString, _parser.Text);
            _parser.SkipTo("1");
            Assert.AreEqual("ab1ab", _parser.View);
            Assert.AreEqual(5, _parser.Length);
            Assert.AreEqual(2, _parser.Index);
            Assert.AreEqual(TestString, _parser.Text);
            Assert.AreEqual("ab", _parser.Left);
            Assert.AreEqual("1ab", _parser.Right);
            _parser.SkipTo("2");
            Assert.IsTrue(_parser.End);
            src = src.TrimToEndOf("2");
            Assert.AreEqual("ab1ab2", src.View);
            Assert.AreEqual(6, src.Length);
            Assert.AreEqual(0, src.Index);
            Assert.AreEqual(TestString, src.Text);
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
            var clone = _parser.Clone();
            Assert.AreEqual("ab", clone.SubstringToAndSkip('1'));
            Assert.AreEqual("1ab", _parser.SkipTo('1').SubstringTo('2'));
            Assert.AreEqual("ab", _parser.SkipToEndOf("ab").SubstringTo("3"));
            Assert.AreEqual("ab", _parser.SkipTo("ab", 3).SubstringTo("4"));
            Assert.AreEqual("3ababab", _parser.SkipToEndOf("ab", 4).SubstringTo("4"));
        }

        [Test]
        public void NextIsTests()
        {
            Assert.AreEqual("ab", _parser.SubstringToAndSkip('1'));
            Assert.IsTrue(_parser.NextIs('a'));
            Assert.IsTrue(_parser.NextIs("ab2"));
            Assert.AreEqual("ab", _parser.SubstringToAndSkip("2ab"));
            Assert.IsTrue(_parser.NextIs("ab3"));
            Assert.IsTrue(_parser.SkipNext("ab"));
            Assert.IsFalse(_parser.SkipNext("ab"));
            Assert.AreEqual('3', _parser.ReadChar());
            Assert.AreEqual("ab1ab", _parser.SkipToEndOf('4').Substring(5));
        }
    }
}