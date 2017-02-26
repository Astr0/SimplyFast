using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplyFast.Strings;

namespace SimplyFast.Log.Internal.Outputs.Writers
{
    public class InfoWriter : IWriter
    {
        private readonly ILogInfoProvider _provider;
        private readonly List<Part> _parts = new List<Part>();

        private struct Part
        {
            public readonly string Name;
            public readonly string Format;
            public bool IsLogInfo => Name != null;

            public Part(string name, string format)
            {
                Name = name;
                Format = format;
            }
        }

        public InfoWriter(ILogInfoProvider provider)
        {
            _provider = provider;
        }

        public void Add(string str)
        {
            _parts.Add(new Part(null, str));
        }

        public void Add(string name, string format)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            _parts.Add(new Part(name, format));
        }

        public void Write(TextWriter writer, IMessage message)
        {
            Write(writer, (ILogInfoProvider)message);
        }

        public void Write(TextWriter writer, ILogInfoProvider message)
        {
            foreach (var part in _parts)
            {
                var format = part.Format;
                if (part.IsLogInfo)
                {
                    var info = message.Get(part.Name) ?? _provider.Get(part.Name);
                    if (info != null)
                        writer.Write(info.ToString(format));
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
    }
}