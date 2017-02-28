using SimplyFast.Log.Internal;
using SimplyFast.Strings.Tokens;

namespace SimplyFast.Log
{
    public static class MessageEx
    {
        public static IMessage Default(Severity severity)
        {
            return new DefaultMessage(severity);
        }

        public static IMessage Default(Severity severity, IStringTokenStorage tokenStorage)
        {
            return new DefaultMessage(severity, tokenStorage);
        }
    }
}