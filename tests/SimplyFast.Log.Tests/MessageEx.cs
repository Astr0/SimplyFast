using SimplyFast.Log.Messages;

namespace SimplyFast.Log.Tests
{
    public static class MessageEx
    {
        private static readonly IMessageFactory _messageFactory = LoggerEx.CreateDefaultMessageFactory();

        public static IMessage Default(Severity severity, string msg = null, ILogger logger = null)
        {
            return _messageFactory.CreateMessage(logger, severity, () => msg);
        }
    }
}