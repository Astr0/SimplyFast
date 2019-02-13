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
        public static IMessage CustomMessage(Severity severity, Func<IMessage, MessageToken, string, string> get)
        {
            return new CustomMessage(severity, get);
        }

        public static IMessageFactory CreateDefaultMessageFactory()
        {
            var factory = new DefaultMessageFactory();
            AddDefaultResolvers(factory);
            return factory;
        }

        private static void AddDefaultResolvers(IMessageFactory factory)
        {
            factory.Map(MessageTokens.Date, (m, f) => DateTime.Now.ToString(f));
            factory.Map(MessageTokens.NewLine, (m, f) => Environment.NewLine);
            factory.Map(MessageTokens.AppData, (m, f) => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            factory.Map(MessageTokens.AppRoot, (m, f) => AppEx.ExecutableDirectory);
            factory.Map(MessageTokens.Thread, (m, f) =>
            {
                var current = System.Threading.Thread.CurrentThread;
                return current.Name ?? current.ManagedThreadId.ToString();
            });
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