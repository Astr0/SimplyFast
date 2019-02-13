using System;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.Log.Messages;

namespace SimplyFast.Log
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class SeverityEx
    {
        public static bool ShouldLog(this Severity severity, IMessage message)
        {
            return severity.ShouldLog(message.Severity);
        }

        public static bool ShouldLog(this Severity severity, Severity messageSeverity)
        {
            return severity <= messageSeverity && messageSeverity < Severity.Off;
        }

        private static string ToStrU(this Severity severity)
        {
            switch (severity)
            {
                case Severity.Trace:
                    return "TRACE";
                case Severity.Debug:
                    return "DEBUG";
                case Severity.Info:
                    return "INFO";
                case Severity.Warn:
                    return "WARN";
                case Severity.Error:
                    return "ERROR";
                case Severity.Fatal:
                    return "FATAL";
                case Severity.Off:
                    return "OFF";
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }

        private static string ToStrL(this Severity severity)
        {
            switch (severity)
            {
                case Severity.Trace:
                    return "Trace";
                case Severity.Debug:
                    return "Debug";
                case Severity.Info:
                    return "Info";
                case Severity.Warn:
                    return "Warn";
                case Severity.Error:
                    return "Error";
                case Severity.Fatal:
                    return "Fatal";
                case Severity.Off:
                    return "Off";
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }

        public static string ToStr(this Severity severity, string format = null)
        {
            if (format == null)
                return severity.ToStrL();
            switch (format)
            {
                case "u":
                case "U":
                    return severity.ToStrU();
                case "l":
                case "L":
                case "G":
                case "g":
                case "":
                case "F":
                case "f":
                    return severity.ToStrL();
                default:
                    return severity.ToString(format);
            }
        }
    }
}