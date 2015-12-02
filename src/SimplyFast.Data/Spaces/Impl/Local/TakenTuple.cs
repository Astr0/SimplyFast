namespace SF.Data.Spaces
{
    internal struct TakenTuple<T> where T : class
    {
        public LocalSpaceTableImpl<T> TableImpl;
        public readonly T Tuple;

        public TakenTuple(LocalSpaceTableImpl<T> tableImpl, T tuple)
        {
            TableImpl = tableImpl;
            Tuple = tuple;
        }
    }
}