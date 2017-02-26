using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.Log
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class LoggerEx
    {
        public static void Log(this ILogger logger, Severity severity, params ILogInfo[] info)
        {
            logger.Log(severity, info);
        }

        public static void Log(this ILogger logger, Severity severity, string format, params object[] args)
        {
            logger.Log(severity, LogInfoEx.Message(format, args));
        }

        public static void Debug(this ILogger logger, string format, params object[] args)
        {
            logger.Log(Severity.Debug, format, args);
        }

        public static void Info(this ILogger logger, string format, params object[] args)
        {
            logger.Log(Severity.Info, format, args);
        }

        public static void Warning(this ILogger logger, string format, params object[] args)
        {
            logger.Log(Severity.Warn, format, args);
        }

        public static void Error(this ILogger logger, string format, params object[] args)
        {
            logger.Log(Severity.Error, format, args);
        }

        public static void Fatal(this ILogger logger, string format, params object[] args)
        {
            logger.Log(Severity.Fatal, format, args);
        }
    }
}