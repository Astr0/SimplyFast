using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.Log
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class SeverityEx
    {
        public static bool ShouldLog(this Severity logSeverity, IMessage message)
        {
            return logSeverity.ShouldLog(message.Severity);
        }

        public static bool ShouldLog(this Severity logSeverity, Severity messageSeverity)
        {
            return logSeverity <= messageSeverity && logSeverity < Severity.Off;
        }

       
        public static string ToStr(this Severity severity, string format = null)
        {
            if (format == null)
                return severity.ToString();
            switch (format)
            {
                case "u":
                case "U":
                    return severity.ToString().ToUpperInvariant();
                case "l":
                case "L":
                    return severity.ToString().ToLowerInvariant();
                default:
                    return severity.ToString(format);
            }
        }
    }
}