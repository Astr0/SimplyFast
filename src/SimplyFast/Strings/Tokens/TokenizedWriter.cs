using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimplyFast.Strings.Tokens
{
    public class TokenizedWriter: IEnumerable<TokenizedWriter.Token>
    {
        private readonly List<Token> _tokens = new List<Token>();

        public struct Token
        {
            public readonly string Name;
            public readonly string Format;
            public bool IsStringToken => Name != null;

            public Token(string name, string format)
            {
                Name = name;
                Format = format;
            }
        }

        public void Add(string str)
        {
            _tokens.Add(new Token(null, str));
        }

        public void Add(string tokenName, string format)
        {
            if (tokenName == null)
                throw new ArgumentNullException(nameof(tokenName));
            _tokens.Add(new Token(tokenName, format));
        }

        public void Write(TextWriter writer, Func<string, IStringToken> resolver)
        {
            foreach (var part in _tokens)
            {
                var format = part.Format;
                if (part.IsStringToken)
                {
                    var info = resolver(part.Name);
                    if (info != null)
                        writer.Write(info.ToString(format));
                    else
                    {
                        writer.Write('%');
                        writer.Write(part.Name);
                        if (format != null)
                        {
                            writer.Write(':');
                            writer.Write(format);
                        }
                        writer.Write('%');
                    }
                }
                else if (format != null)
                {
                    writer.Write(format);
                }
            }
        }

        private void AddTokenString(string tokenString)
        {
            var sp = new StringParser(tokenString);
            var name = sp.SubstringTo(':');
            if (string.IsNullOrEmpty(name))
                return;
            sp.Skip(1);
            var format = sp.Right;
            if (string.IsNullOrEmpty(format))
                format = null;
            Add(name, format);
        }

        public void AddTokenizedString(string tokenizedString)
        {
            var sp = new StringParser(tokenizedString);
            var inToken = false;
            var sb = new StringBuilder();
            while (!sp.End)
            {
                var str = sp.SubstringTo('%');
                sp.Skip(1);
                sb.Append(str);
                if (sp.NextIs('%') && !inToken)
                {
                    // append % for %%
                    sb.Append('%');
                    sp.Skip(1);
                }
                else
                {
                    // end of something
                    var current = sb.ToString();
                    sb.Clear();
                    if (!string.IsNullOrEmpty(current))
                    {
                        if (inToken)
                            AddTokenString(current);
                        else
                            Add(current);
                    }
                    inToken = !inToken;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Token> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }
    }
}