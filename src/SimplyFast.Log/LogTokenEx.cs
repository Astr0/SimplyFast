using System.Diagnostics.CodeAnalysis;
using SimplyFast.Strings.Tokens;
using static SimplyFast.Strings.Tokens.StringTokenEx;
using DateTime = System.DateTime;

namespace SimplyFast.Log
{
    public static class LogTokenEx
    {
        public static IStringToken Now()
        {
            return DateTime(Names.Date, DateTime.Now);
        }

        public static IStringToken Severity(Severity severity)
        {
            return new SeverityToken(severity);
        }

        public static IStringToken Logger(ILogger logger)
        {
            return String(Names.Logger, logger.ToString());
        }

        public static IStringToken Message(string message)
        {
            return String(Names.Message, message);
        }

        public static IStringToken Message(string format, params object[] args)
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

        private abstract class StringToken : IStringToken
        {
            public abstract string Name { get; }
            public abstract string ToString(string format);

            public override string ToString()
            {
                return ToString(null);
            }
        }

        private class SeverityToken : StringToken
        {
            private readonly Severity _severity;

            public SeverityToken(Severity severity)
            {
                _severity = severity;
            }

            public override string Name => Names.Severity;

            public override string ToString(string format)
            {
                if (format == null)
                    return _severity.ToString();
                switch (format)
                {
                    case "u":
                    case "U":
                        return _severity.ToString().ToUpperInvariant();
                    case "l":
                    case "L":
                        return _severity.ToString().ToLowerInvariant();
                    default:
                        return _severity.ToString(format);
                }
            }
        }
    }
}