using System;
using System.Collections;
using System.Collections.Generic;

namespace SimplyFast.Strings.Tokens
{
    public static partial class StringTokenProviderEx
    {
        private class DictionaryStorage : IStringTokenStorage
        {
            private readonly Dictionary<string, IStringToken> _infos;

            public DictionaryStorage()
            {
                _infos = new Dictionary<string, IStringToken>(StringTokenEx.NameComparer);
            }

            public void Add(IStringToken info)
            {
                if (info == null)
                    throw new ArgumentNullException(nameof(info));
                _infos.Add(info.Name, info);
            }

            public void Upsert(IStringToken info)
            {
                if (info == null)
                    throw new ArgumentNullException(nameof(info));
                _infos[info.Name] = info;
            }

            public IEnumerator<IStringToken> GetEnumerator()
            {
                return _infos.Values.GetEnumerator();
            }

            public IStringToken Get(string name)
            {
                return _infos.TryGetValue(name, out IStringToken info) ? info : null;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}