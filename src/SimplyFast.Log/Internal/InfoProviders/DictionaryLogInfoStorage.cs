using System;
using System.Collections;
using System.Collections.Generic;

namespace SimplyFast.Log.Internal.InfoProviders
{
    internal class DictionaryLogInfoStorage: ILogInfoStorage
    {
        private readonly Dictionary<string, ILogInfo> _infos;

        public DictionaryLogInfoStorage()
        {
            _infos = new Dictionary<string, ILogInfo>(LogInfoEx.NameComparer);
        }

        public void Add(ILogInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            _infos.Add(info.Name, info);
        }

        public void Upsert(ILogInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            _infos[info.Name] = info;
        }

        public IEnumerator<ILogInfo> GetEnumerator()
        {
            return _infos.Values.GetEnumerator();
        }

        public ILogInfo Get(string name)
        {
            return _infos.TryGetValue(name, out ILogInfo info) ? info : null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}