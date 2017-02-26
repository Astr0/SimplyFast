using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.Log
{
    public static class LogInfoEx
    {
        public static readonly IEqualityComparer<string> NameComparer = StringComparer.OrdinalIgnoreCase;

        public static bool NameEquals(this ILogInfo logInfo, string name)
        {
            return NameComparer.Equals(logInfo.Name, name);
        }

        public static ILogInfo DateTime(string name, DateTime value)
        {
            return new DateTimeInfo(name, value);
        }

        public static ILogInfo Now()
        {
            return DateTime(Names.Date, System.DateTime.Now);
        }

        public static ILogInfo FormatString(string name, string format, object[] args)
        {
            return new FormatStringInfo(name, format, args);
        }

        public static ILogInfo Func(string name, Func<string, string> toString)
        {
            return new FuncInfo(name, toString);
        }

        public static ILogInfo Severity(Severity severity)
        {
            return new SeverityInfo(severity);
        }

        public static ILogInfo String(string name, string value)
        {
            return new StringInfo(name, value);
        }

        public static ILogInfo Logger(ILogger logger)
        {
            return String(Names.Logger, logger.ToString());
        }

        public static ILogInfo Message(string message)
        {
            return String(Names.Message, message);
        }

        public static ILogInfo Message(string format, params object[] args)
        {
            if (args == null || args.Length == 0)
                return Message(format);
            return FormatString(Names.Message, format, args);
        }

        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
        public static class Names
        {
            public const string Logger = "logger";
            public const string Severity = "severity";
            public const string Date = "date";
            public const string Message = "message";
        }

        private abstract class LogInfo : ILogInfo
        {
            public abstract string Name { get; }
            public abstract string ToString(string format);

            public override string ToString()
            {
                return ToString(null);
            }
        }

        private class StringInfo : LogInfo
        {
            private readonly string _value;

            public StringInfo(string name, string value)
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

        private class SeverityInfo : LogInfo
        {
            private readonly Severity _severity;

            public SeverityInfo(Severity severity)
            {
                _severity = severity;
            }

            public override string Name => Names.Severity;

            public override string ToString(string format)
            {
                return _severity.ToStr(format);
            }
        }

        private class FuncInfo : LogInfo
        {
            private readonly Func<string, string> _toString;

            public FuncInfo(string name, Func<string, string> toString)
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

        private class DateTimeInfo : LogInfo
        {
            private readonly DateTime _date;

            public DateTimeInfo(string name, DateTime date)
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

        private class FormatStringInfo : LogInfo
        {
            private readonly object[] _args;
            private readonly string _format;
            private string _value;

            public FormatStringInfo(string name, string format, object[] args)
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