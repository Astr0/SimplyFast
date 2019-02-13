using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using SimplyFast.Log.Messages;
using SimplyFast.Strings;

namespace SimplyFast.Log.Internal
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal class TokenStringWriter : IWriter
    {
        private readonly List<Func<IMessage, string>> _patterns = new List<Func<IMessage, string>>();

        public void Write(TextWriter writer, IMessage message)
        {
            foreach (var pattern in _patterns)
            {
                var value = pattern(message);
                if (value != null)
                    writer.Write(value);
            }
        }

        private void Add(Func<IMessage, string> part)
        {
            _patterns.Add(part);
        }

        private static Func<IMessage, string> ParseToken(string str)
        {
            var sp = new StringParser(str);
            var token = sp.SubstringTo(':');
            if (string.IsNullOrEmpty(token))
                return null;
            sp.Skip(1);
            var format = sp.Right;
            if (string.IsNullOrEmpty(format))
                format = null;
            var messageToken = MessageTokens.Get(token);
            return m => m.Get(messageToken, format);
        }

        public static TokenStringWriter Parse(string stringWithTokens)
        {
            var sp = new StringParser(stringWithTokens);
            var result = new TokenStringWriter();
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
                        var pattern = inToken
                            ? ParseToken(current)
                            : m => current;
                        if (pattern != null)
                            result.Add(pattern);
                    }

                    inToken = !inToken;
                }
            }

            return result;
        }
    }
}