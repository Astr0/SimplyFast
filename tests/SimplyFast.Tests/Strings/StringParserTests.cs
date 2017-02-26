using Xunit;
using SimplyFast.Strings;

namespace SimplyFast.Tests.Strings
{
    
    public class StringParserTests
    {
        public StringParserTests()
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

        [Fact]
        public void BackTests()
        {
            _parser.Index = _parser.Length;
            Assert.Equal(TestString.Length - 1, _parser.BackTo("4").Index);
            Assert.Equal(TestString.Length - 1, _parser.BackToEndOf("ab").Index);
            Assert.Equal("abab", _parser.BackToEndOf("ab", 3).SubstringTo("4"));
            Assert.Equal("ababab", _parser.BackTo("ab", 3).SubstringTo("4"));
            Assert.Equal("ababab", _parser.BackToEndOf("3").SubstringTo("4"));
        }

        [Fact]
        public void TrimTests()
        {
            var src = _parser.Clone();
            _parser = _parser.TrimTo("2");
            Assert.Equal("ab1ab", _parser.View);
            Assert.Equal(5, _parser.Length);
            Assert.Equal(0, _parser.Index);
            Assert.Equal(TestString, _parser.Text);
            _parser.SkipTo("1");
            Assert.Equal("ab1ab", _parser.View);
            Assert.Equal(5, _parser.Length);
            Assert.Equal(2, _parser.Index);
            Assert.Equal(TestString, _parser.Text);
            Assert.Equal("ab", _parser.Left);
            Assert.Equal("1ab", _parser.Right);
            _parser.SkipTo("2");
            Assert.True(_parser.End);
            src = src.TrimToEndOf("2");
            Assert.Equal("ab1ab2", src.View);
            Assert.Equal(6, src.Length);
            Assert.Equal(0, src.Index);
            Assert.Equal(TestString, src.Text);
        }

        [Fact]
        public void BaseTests()
        {
            Assert.Equal(TestString, _parser.Text);
            Assert.Equal(TestString.Length, _parser.Length);
            Assert.Equal("ab", _parser.SubstringTo("1"));
            Assert.Equal("1ab2abab3", _parser.SubstringTo("ababab4"));
            Assert.Equal("ab1ab2abab3", _parser.Left);
            Assert.Equal("ababab4ab1ab2abab3ababab4", _parser.Right);
        }

        [Fact]
        public void CloneTests()
        {
            BaseTests();
            var clone = _parser.Clone();
            Assert.Equal(_parser.Text, clone.Text);
            Assert.Equal(_parser.Length, clone.Length);
            Assert.Equal(_parser.Index, clone.Index);
            _parser = clone;
            _parser.Reset();
            FullTests();
        }

        [Fact]
        public void MoveTests()
        {
            Assert.Equal(0, _parser.Index);
            Assert.True(_parser.Start);
            Assert.False(_parser.End);
            _parser.Skip(1);
            Assert.Equal(1, _parser.Index);
            Assert.False(_parser.Start);
            Assert.False(_parser.End);
            _parser.Skip(1000);
            Assert.Equal(TestString.Length, _parser.Index);
            Assert.False(_parser.Start);
            Assert.True(_parser.End);
            _parser.Back(1);
            Assert.Equal(TestString.Length - 1, _parser.Index);
            Assert.False(_parser.Start);
            Assert.False(_parser.End);
            _parser.Back(1000);
            Assert.True(_parser.Start);
            Assert.False(_parser.End);
            Assert.Equal(0, _parser.Index);
            _parser.Index = TestString.Length / 2;
            Assert.False(_parser.Start);
            Assert.False(_parser.End);
            Assert.Equal(TestString.Length / 2, _parser.Index);
        }

        [Fact]
        public void ResetTests()
        {
            BaseTests();
            _parser.Reset();
            FullTests();
        }


        [Fact]
        public void SkipTests()
        {
            var clone = _parser.Clone();
            Assert.Equal("ab", clone.SubstringToAndSkip('1'));
            Assert.Equal("1ab", _parser.SkipTo('1').SubstringTo('2'));
            Assert.Equal("ab", _parser.SkipToEndOf("ab").SubstringTo("3"));
            Assert.Equal("ab", _parser.SkipTo("ab", 3).SubstringTo("4"));
            Assert.Equal("3ababab", _parser.SkipToEndOf("ab", 4).SubstringTo("4"));
        }

        [Fact]
        public void NextIsTests()
        {
            Assert.Equal("ab", _parser.SubstringToAndSkip('1'));
            Assert.True(_parser.NextIs('a'));
            Assert.True(_parser.NextIs("ab2"));
            Assert.Equal("ab", _parser.SubstringToAndSkip("2ab"));
            Assert.True(_parser.NextIs("ab3"));
            Assert.True(_parser.SkipNext("ab"));
            Assert.False(_parser.SkipNext("ab"));
            Assert.Equal('3', _parser.ReadChar());
            Assert.Equal("ab1ab", _parser.SkipToEndOf('4').Substring(5));
        }
    }
}