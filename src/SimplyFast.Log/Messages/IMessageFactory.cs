using System;

namespace SimplyFast.Log.Messages
{
    public interface IMessageFactory
    {
        IMessage CreateMessage(ILogger source, Severity severity, Func<string> getMessage);

        void Map(MessageToken token, MessageTokenResolver<IMessage> resolver);
        string Get(IMessage message, MessageToken token, string format);
    }
}