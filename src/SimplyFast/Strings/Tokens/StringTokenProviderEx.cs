using System.Collections.Generic;

namespace SimplyFast.Strings.Tokens
{
    public static partial class StringTokenProviderEx
    {
        public static IStringTokenStorage Indexed()
        {
            return new DictionaryStorage();
        }

        public static IStringTokenStorage Sequential()
        {
            return new ListStorage();
        }

        public static IStringTokenProvider Combine(IEnumerable<IStringTokenProvider> providers)
        {
            return new CombineProviders(providers);
        }

        public static IStringTokenProvider Combine(params IStringTokenProvider[] providers)
        {
            return Combine((IEnumerable<IStringTokenProvider>)providers);
        }
    }
}