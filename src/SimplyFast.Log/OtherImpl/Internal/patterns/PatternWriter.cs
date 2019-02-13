using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using SimplyFast.Log.Messages;
using SimplyFast.Strings;

namespace SimplyFast.Log.Internal
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class PatternWriter : IWriter
    {
        private readonly List<IOutputPattern> _patterns = new List<IOutputPattern>();

        public IEnumerable<IOutputPattern> Patterns => _patterns;

        public void Write(TextWriter writer, IMessage message)
        {
            foreach (var pattern in _patterns)
            {
                var value = pattern.GetValue(message);
                if (value != null)
                    writer.Write(value);
            }
        }

        private void Add(IOutputPattern pattern)
        {
            _patterns.Add(pattern);
        }

        public void Add(string str)
        {
            Add(new StringOutputPattern(str));
        }

        public void Add(MessageToken token, string format = null)
        {
            Add(new TokenOutputPattern(token, format));
        }

        public static PatternWriter Parse(string tokenizedString)
        {
            var sp = new StringParser(tokenizedString);
            var result = new PatternWriter();
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
                            ? TokenOutputPattern.ParseToken(current)
                            : new StringOutputPattern(current);
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