using System;

namespace SimplyFast.Log.Messages.Internal
{
    internal class DefaultMessageFactory : IMessageFactory
    {
        private readonly MessageTokenResolvers<IMessage> _tokenResolvers = new MessageTokenResolvers<IMessage>();

        public IMessage CreateMessage(ILogger source, Severity severity, Func<string> getMessage)
        {
            return new DefaultMessage(this, source, severity, getMessage);
        }

        public void Map(MessageToken token, MessageTokenResolver<IMessage> resolver)
        {
            _tokenResolvers.Add(token, resolver);
        }

        public string Get(IMessage message, MessageToken token, string format)
        {
            return _tokenResolvers.GetTokenValue(message, token, format);
        }
    }
}