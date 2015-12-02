namespace SF.Data.Legacy.Spaces
{
    internal interface ILocalSpaceTable
    {
        void EnsureTransactionsCapacity(int count);
        void CommitTransaction(LocalTransaction transaction);
        void AbortTransaction(LocalTransaction transaction);
    }
}