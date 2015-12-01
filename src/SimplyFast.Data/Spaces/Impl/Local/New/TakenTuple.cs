namespace SF.Data.Spaces.New
{
    public struct TakenTuple<T> where T : class
    {
        public readonly LocalSpaceTableImpl<T> TableImpl;
        public readonly T Tuple;

        public TakenTuple(LocalSpaceTableImpl<T> tableImpl, T tuple)
        {
            TableImpl = tableImpl;
            Tuple = tuple;
        }
    }
}