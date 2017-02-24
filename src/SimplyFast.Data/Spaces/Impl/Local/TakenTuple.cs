using System.Runtime.CompilerServices;

namespace SimplyFast.Data.Spaces.Impl.Local
{
    internal struct TakenTuple<T> 
    {
        public readonly LocalTable<T> Table;
        public readonly T Tuple;

        public TakenTuple(LocalTable<T> table, T tuple)
        {
            Table = table;
            Tuple = tuple;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Abort()
        {
            Table.Add(Tuple);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AbortToRoot()
        {
            if (Table.HierarchyLevel == 0)
                Abort();
        }
    }
}