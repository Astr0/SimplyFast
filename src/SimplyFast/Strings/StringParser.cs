using System;
using System.Diagnostics.CodeAnalysis;

namespace SF.Strings
{
    /// <summary>
    ///     Useful class for string parsing
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class StringParser : ICloneable
    {
        private int _index;
        private readonly string _text;
        private int _length;

        public StringParser(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            _text = text;
            _length = text.Length;
        }

        private StringParser(string text, int length)
        {
            _text = text;
            _length = length;
        }

        public int Length => _length;

        public string Text => _text;

        public int Index
        {
            get { return _index; }
            set { _index = Math.Max(0, Math.Min(value, _length)); }
        }

        public bool End => _index == _length;

        public bool Start => _index == 0;

        public string View => _text.Substring(0, _length);
        
        public string Right => End ? string.Empty : _text.Substring(_index, _length - _index);

        public string Left => Start ? string.Empty : _text.Left(_index);

        public int CharactersLeft => _length - _index;

        object ICloneable.Clone()
        {
            return Clone();
        }

        public void Reset()
        {
            _index = 0;
            _length = _text.Length;
        }

        public StringParser Skip(int count)
        {
            _index = (_index + count).Clip(0, _length);
            return this;
        }

        public StringParser Back(int count)
        {
            return Skip(-count);
        }

        public StringParser SkipTo(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (End)
                return this;
            var pos = _text.IndexOf(str, _index, CharactersLeft, StringComparison.Ordinal);
            _index = pos < 0 ? _length : pos;
            return this;
        }

        public StringParser SkipTo(char c)
        {
            if (End)
                return this;
            var pos = _text.IndexOf(c, _index, CharactersLeft);
            _index = pos < 0 ? _length : pos;
            return this;
        }

        public StringParser TrimTo(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (End)
                return this;
            var pos = _text.IndexOf(str, _index, CharactersLeft, StringComparison.Ordinal);
            if (pos >= 0)
                _length = pos;
            return this;
        }

        public StringParser BackTo(string str)
        {
            return BackToEndOf(str).Back(str.Length);
        }

        public StringParser SkipToEndOf(string str)
        {
            return SkipTo(str).Skip(str.Length);
        }

        public StringParser SkipToEndOf(char c)
        {
            return SkipTo(c).Skip(1);
        }

        public StringParser TrimToEndOf(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (End)
                return this;
            var pos = _text.IndexOf(str, _index, CharactersLeft, StringComparison.Ordinal);
            if (pos >= 0)
                _length = pos + str.Length;
            return this;
        }


        public StringParser BackToEndOf(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            //Back(str._length - 1);
            if (Start)
                return this;
            var pos = _text.LastIndexOf(str, _index, _index, StringComparison.Ordinal);
            _index = pos < 0 ? 0 : pos + str.Length;
            return this;
        }

        public StringParser SkipTo(string str, int count)
        {
            for (var i = 0; i < count - 1; i++)
            {
                SkipToEndOf(str);
            }
            return SkipTo(str);
        }

        public StringParser BackTo(string str, int count)
        {
            for (var i = 0; i < count - 1; i++)
            {
                BackTo(str);
                Back(1);
            }
            return BackTo(str);
        }


        public StringParser SkipToEndOf(string str, int count)
        {
            SkipTo(str, count);
            return Skip(str.Length);
        }

        public StringParser BackToEndOf(string str, int count)
        {
            BackTo(str, count - 1);
            return BackToEndOf(str);
        }

        public string SubstringTo(string str)
        {
            if (End)
                return string.Empty;
            var oldIndex = _index;
            SkipTo(str);
            return _text.Substring(oldIndex, _index - oldIndex);
        }

        public string SubstringToAndSkip(string str)
        {
            var res = SubstringTo(str);
            Skip(str.Length);
            return res;
        }

        public string SubstringTo(char c)
        {
            if (End)
                return string.Empty;
            var oldIndex = _index;
            SkipTo(c);
            return _text.Substring(oldIndex, _index - oldIndex);
        }

        public string SubstringToAndSkip(char c)
        {
            var res = SubstringTo(c);
            Skip(1);
            return res;
        }

        public char ReadChar()
        {
            return End ? '\0' : Text[_index++];
        }

        public bool NextIs(char c)
        {
            if (End)
                return false;
            // ReSharper disable once LoopCanBeConvertedToQuery
            return Text[_index] == c;
        }

        public bool NextIs(string str)
        {
            if (_index + str.Length > Length)
                return false;
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < str.Length; i++)
                if (str[i] != Text[_index + i])
                    return false;
            return true;
        }

        public bool SkipNext(string toSkip)
        {
            if (!NextIs(toSkip))
                return false;
            Skip(toSkip.Length);
            return true;
        }

        public override string ToString()
        {
            return Left + "|" + Right;
        }

        public string Substring(int count)
        {
            if (End)
                return string.Empty;
            var result = Text.Substring(_index, count);
            Skip(count);
            return result;
        }

        public StringParser Clone()
        {
            return new StringParser(_text, _length) { _index = _index };
        }
    }
}