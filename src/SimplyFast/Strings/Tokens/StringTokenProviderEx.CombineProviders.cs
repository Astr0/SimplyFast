using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplyFast.Strings.Tokens
{
    public static partial class StringTokenProviderEx
    {
        private class CombineProviders : IStringTokenProvider
        {
            private readonly IStringTokenProvider[] _providers;

            public CombineProviders(IEnumerable<IStringTokenProvider> resolvers)
            {
                _providers = resolvers.ToArray();
            }

            public IEnumerator<IStringToken> GetEnumerator()
            {
                return _providers.SelectMany(x => x).GetEnumerator();
            }

            public IStringToken Get(string name)
            {
                return _providers.Select(x => x.Get(name)).FirstOrDefault(x => x != null);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}