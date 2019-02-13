using System;
using SimplyFast.Log.Messages;

namespace SimplyFast.Log
{
    public interface IMessageFactory
    {
        IMessage CreateMessage(ILogger source, Severity severity, Func<string> getMessage);

        //void MapToken(MessageToken token, MessageTokenResolver<IMessage> resolver);
        string GetTokenValue(IMessage message, MessageToken token, string format);
    }
}