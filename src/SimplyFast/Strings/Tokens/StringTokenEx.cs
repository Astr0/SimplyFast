using System;
using System.Collections.Generic;

namespace SimplyFast.Strings.Tokens
{
    public static class StringTokenEx
    {
        public static readonly IEqualityComparer<string> NameComparer = StringComparer.OrdinalIgnoreCase;

        public static bool NameEquals(this IStringToken token, string name)
        {
            return NameComparer.Equals(token.Name, name);
        }

        public static IStringToken DateTime(string name, DateTime value)
        {
            return new DateTimeToken(name, value);
        }

        
        public static IStringToken FormatString(string name, string format, object[] args)
        {
            return new FormatStringToken(name, format, args);
        }

        public static IStringToken Func(string name, Func<string, string> toString)
        {
            return new FuncToken(name, toString);
        }

        
        public static IStringToken String(string name, string value)
        {
            return new ConstStringToken(name, value);
        }

        
        private abstract class StringToken : IStringToken
        {
            public abstract string Name { get; }
            public abstract string ToString(string format);

            public override string ToString()
            {
                return ToString(null);
            }
        }

        private class ConstStringToken : StringToken
        {
            private readonly string _value;

            public ConstStringToken(string name, string value)
            {
                _value = value;
                Name = name;
            }

            public override string Name { get; }

            public override string ToString(string format)
            {
                return _value;
            }
        }

        

        private class FuncToken : StringToken
        {
            private readonly Func<string, string> _toString;

            public FuncToken(string name, Func<string, string> toString)
            {
                _toString = toString;
                Name = name;
            }

            public override string Name { get; }

            public override string ToString(string format)
            {
                return _toString(format);
            }
        }

        private class DateTimeToken : StringToken
        {
            private readonly DateTime _date;

            public DateTimeToken(string name, DateTime date)
            {
                Name = name;
                _date = date;
            }

            public override string Name { get; }

            public override string ToString(string format)
            {
                return _date.ToString(format);
            }
        }

        private class FormatStringToken : StringToken
        {
            private readonly object[] _args;
            private readonly string _format;
            private string _value;

            public FormatStringToken(string name, string format, object[] args)
            {
                Name = name;
                _format = format;
                _args = args;
            }

            public override string Name { get; }

            public override string ToString(string format)
            {
                return _value ?? (_value = string.Format(_format, _args));
            }
        }
    }
}