using System.Collections;
using System.Collections.Generic;

namespace SimplyFast.Log.Messages.Internal
{
    public class GlobalTokenResolvers : IEnumerable<KeyValuePair<MessageToken, GlobalTokenResolver>>
    {
        private readonly Dictionary<MessageToken, GlobalTokenResolver> _resolvers =
            new Dictionary<MessageToken, GlobalTokenResolver>();

        public IEnumerator<KeyValuePair<MessageToken, GlobalTokenResolver>> GetEnumerator()
        {
            return _resolvers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(MessageToken token, GlobalTokenResolver resolver)
        {
            _resolvers[token] = resolver;
        }

        public string GetTokenValue(MessageToken token, string format)
        {
            return _resolvers.TryGetValue(token, out var resolver) ? resolver(format) : null;
        }
    }
}