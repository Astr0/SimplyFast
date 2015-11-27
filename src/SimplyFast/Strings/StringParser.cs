using System;

namespace SF.Strings
{
    /// <summary>
    ///     Useful class for string parsing
    /// </summary>
    public class StringParser : ICloneable
    {
        private int _index;

        public StringParser(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            Text = text;
            Length = Text.Length;
        }

        public int Length { get; private set; }
        public string Text { get; private set; }

        public int Index
        {
            get { return _index; }
            set { _index = Math.Max(0, Math.Min(value, Length)); }
        }

        public bool End
        {
            get { return _index == Length; }
        }

        public bool Start
        {
            get { return _index == 0; }
        }

        public string Right
        {
            get { return End ? string.Empty : Text.Substring(_index); }
        }

        public string Left
        {
            get { return Start ? string.Empty : Text.Left(_index); }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public void Reset()
        {
            _index = 0;
        }

        public StringParser Skip(int count)
        {
            _index = (_index + count).Clip(0, Length);
            return this;
        }

        public StringParser Back(int count)
        {
            return Skip(-count);
        }

        public StringParser SkipTo(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            if (End)
                return this;
            var pos = Text.IndexOf(str, _index, StringComparison.Ordinal);
            _index = pos < 0 ? Length : pos;
            return this;
        }

        public StringParser BackTo(string str)
        {
            Skip(str.Length);
            BackToEndOf(str);
            return Back(str.Length);
        }


        public StringParser SkipToEndOf(string str)
        {
            SkipTo(str);
            return Skip(str.Length);
        }

        public StringParser BackToEndOf(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            Back(str.Length - 1);
            if (Start)
                return this;
            var pos = Text.LastIndexOf(str, _index, StringComparison.Ordinal);
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
            return Text.Substring(oldIndex, _index - oldIndex);
        }

        public StringParser Clone()
        {
            return new StringParser(Text) { _index = _index };
        }
    }
}