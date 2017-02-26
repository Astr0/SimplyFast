using System.Collections.Generic;
using SimplyFast.Log.Internal.InfoProviders;

namespace SimplyFast.Log
{
    public static class LogInfoProviderEx
    {
        public static ILogInfoStorage Indexed()
        {
            return new DictionaryLogInfoStorage();
        }

        public static ILogInfoStorage Sequential()
        {
            return new ListLogInfoStorage();
        }

        public static ILogInfoProvider Combine(IEnumerable<ILogInfoProvider> providers)
        {
            return new LogInfoProviders(providers);
        }

        public static ILogInfoProvider Combine(params ILogInfoProvider[] providers)
        {
            return Combine((IEnumerable<ILogInfoProvider>)providers);
        }
    }
}