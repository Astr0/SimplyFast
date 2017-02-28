using System;
using System.Collections;
using System.Collections.Generic;

namespace SimplyFast.Strings.Tokens
{
    public static partial class StringTokenProviderEx
    {
        private class ListStorage : IStringTokenStorage
        {
            private readonly List<IStringToken> _infos;

            public ListStorage() : this(new List<IStringToken>())
            {
            }

            public ListStorage(List<IStringToken> infos)
            {
                if (infos == null)
                    throw new ArgumentNullException(nameof(infos));
                _infos = infos;
            }

            public void Add(IStringToken info)
            {
                if (info == null)
                    throw new ArgumentNullException(nameof(info));
                _infos.Add(info);
            }

            public IEnumerator<IStringToken> GetEnumerator()
            {
                return _infos.GetEnumerator();
            }

            public IStringToken Get(string name)
            {
                return _infos.Find(x => x.NameEquals(name));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Upsert(IStringToken info)
            {
                if (info == null)
                    throw new ArgumentNullException(nameof(info));
                var index = _infos.FindIndex(x => x.NameEquals(info.Name));
                if (index < 0)
                    _infos.Add(info);
                else
                    _infos[index] = info;
            }
        }
    }
}