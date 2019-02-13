using System;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.Log.Internal;
using SimplyFast.Log.Messages;
using SimplyFast.Log.Messages.Internal;

namespace SimplyFast.Log
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class LoggerEx
    {
        public static IMessageFactory CreateDefaultMessageFactory()
        {
            return new DefaultMessageFactory();
        }

        public static ILoggerFactory CreateDefaultFactory(IMessageFactory messageFactory = null, string rootName = null)
        {
            if (messageFactory == null)
                messageFactory = CreateDefaultMessageFactory();
            return new LoggerFactory(new RootLogger(messageFactory, rootName));
        }

        private static IMessage CreateMessage(this ILogger source, Severity severity, Func<string> getMessage)
        {
            return source.MessageFactory.CreateMessage(source, severity, getMessage);
        }

        private static void Log(this ILogger logger, Severity severity, Func<string> getMessage)
        {
            var message = logger.CreateMessage(severity, getMessage);
            logger.Log(message);
        }

        private static void Log(this ILogger logger, Severity severity, string message)
        {
            logger.Log(severity, () => message);
        }

        public static void Log(this ILogger logger, Severity severity, string format, params object[] args)
        {
            if (args == null || args.Length == 0)
                logger.Log(severity, format);
            else
                logger.Log(severity, () => string.Format(format, args));
        }

        public static void Trace(this ILogger logger, string format, params object[] args)
        {
            logger.Log(Severity.Trace, format, args);
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