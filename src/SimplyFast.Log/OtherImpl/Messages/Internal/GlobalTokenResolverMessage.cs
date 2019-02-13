using System.Collections;
using System.Collections.Generic;

namespace SimplyFast.Log.Messages.Internal
{
    public class GlobalTokenResolverMessage : NullMessage, IEnumerable<KeyValuePair<MessageToken, GlobalTokenResolver>>
    {
        private readonly GlobalTokenResolvers _resolvers = new GlobalTokenResolvers();

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
            _resolvers.Add(token, resolver);
        }

        public override string Get(MessageToken token, string format)
        {
            return _resolvers.GetTokenValue(token, format) ?? MessageTokens.GetTokenValue(token, format);
        }
    }
}