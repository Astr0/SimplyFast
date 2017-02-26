using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplyFast.Log.Internal.InfoProviders
{
    internal class LogInfoProviders: ILogInfoProvider
    {
        private readonly ILogInfoProvider[] _providers;

        public LogInfoProviders(IEnumerable<ILogInfoProvider> resolvers)
        {
            _providers = resolvers.ToArray();
        }

        public IEnumerator<ILogInfo> GetEnumerator()
        {
            return _providers.SelectMany(x => x).GetEnumerator();
        }

        public ILogInfo Get(string name)
        {
            return _providers.Select(x => x.Get(name)).FirstOrDefault(x => x != null);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}