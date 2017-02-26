using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplyFast.Log.Internal.InfoProviders
{
    internal class ListLogInfoStorage: ILogInfoStorage
    {
        private readonly List<ILogInfo> _infos;

        public ListLogInfoStorage(): this(new List<ILogInfo>())
        {
        }

        public ListLogInfoStorage(List<ILogInfo> infos)
        {
            if (infos == null)
                throw new ArgumentNullException(nameof(infos));
            _infos = infos;
        }

        public void Add(ILogInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            _infos.Add(info);
        }

        public IEnumerator<ILogInfo> GetEnumerator()
        {
            return _infos.GetEnumerator();
        }

        public ILogInfo Get(string name)
        {
            return _infos.FirstOrDefault(x => x.NameEquals(name));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Upsert(ILogInfo info)
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