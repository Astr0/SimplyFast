namespace SF.Data.Spaces
{
    internal struct TakenTuple<T> where T : class
    {
        public readonly LocalTransaction<T> Transaction;

        public TakenTuple(LocalTransaction<T> transaction, T tuple)
        {
            Transaction = transaction;
            Tuple = tuple;
        }

        public readonly T Tuple;
    }
}