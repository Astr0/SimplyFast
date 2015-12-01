namespace SF.Data.Spaces
{
    internal struct TakenTuple<T> where T : class
    {
        public readonly LocalTransaction Transaction;
        public readonly T Tuple;

        public TakenTuple(LocalTransaction transaction, T tuple)
        {
            Transaction = transaction;
            Tuple = tuple;
        }
    }


}