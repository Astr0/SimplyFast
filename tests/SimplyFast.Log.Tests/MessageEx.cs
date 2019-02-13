using SimplyFast.Log.Messages;

namespace SimplyFast.Log.Tests
{
    public static class MessageEx
    {
        public static IMessage Default(Severity severity, string msg = null)
        {
            return Logger.MessageFactory.CreateMessage(null, severity, () => msg);
        }
    }
}