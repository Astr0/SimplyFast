using System;
using SimplyFast.Data.Spaces.Interface;

namespace SimplyFast.Data.Spaces.Impl.Local
{
    internal class LocalSpace : ISpace
    {
        private ILocalTable[] _tables = new ILocalTable[LocalSpaceConsts.SpaceTablesCapacity];

        public ISpaceProxy CreateProxy()
        {
            return new LocalSpaceProxy(this);
        }

        internal LocalTable<T> GetRootTable<T>(TupleType type)
        {
            if (type.Id >= _tables.Length)
                Array.Resize(ref _tables, type.Id + 1);
            var result = _tables[type.Id];
            if (result != null)
                return (LocalTable<T>)result;
            var table = LocalTable<T>.GetRoot();
            _tables[type.Id] = table;
            return table;
        }
    }
}