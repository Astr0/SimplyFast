namespace SF.Data.Spaces.Local
{
    internal struct TakenTuple<T> 
    {
        public LocalTable<T> Table;
        public readonly T Tuple;

        public TakenTuple(LocalTable<T> table, T tuple)
        {
            Table = table;
            Tuple = tuple;
        }

        public void Abort()
        {
            Table.Add(Tuple);
        }
    }
}