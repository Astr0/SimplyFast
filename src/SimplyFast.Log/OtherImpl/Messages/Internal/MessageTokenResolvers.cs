using System.Collections;
using System.Collections.Generic;

namespace SimplyFast.Log.Messages.Internal
{
    public class MessageTokenResolvers<T> : IEnumerable<KeyValuePair<MessageToken, MessageTokenResolver<T>>>
        where T : IMessage
    {
        private readonly Dictionary<MessageToken, MessageTokenResolver<T>> _resolvers =
            new Dictionary<MessageToken, MessageTokenResolver<T>>();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<MessageToken, MessageTokenResolver<T>>> GetEnumerator()
        {
            return _resolvers.GetEnumerator();
        }

        public void Add(MessageToken token, MessageTokenResolver<T> resolver)
        {
            _resolvers[token] = resolver;
        }

        public string GetTokenValue(T message, MessageToken token, string format)
        {
            MessageTokenResolver<T> resolver;
            return _resolvers.TryGetValue(token, out resolver) ? resolver(message, format) : null;
        }
    }
}