using SimplyFast.Log.Messages;

namespace SimplyFast.Log.Tests
{
    public static class MessageEx
    {
        private static readonly IMessageFactory _messageFactory = LoggerEx.CreateDefaultMessageFactory();

        public static IMessage Default(Severity severity, string msg = null)
        {
            return _messageFactory.CreateMessage(null, severity, () => msg);
        }
    }
}