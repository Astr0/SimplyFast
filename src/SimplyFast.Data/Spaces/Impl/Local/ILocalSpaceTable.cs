namespace SF.Data.Spaces
{
    internal interface ILocalSpaceTable
    {
        void CommitTransaction(LocalTransaction transaction);
        void AbortTransaction(LocalTransaction transaction);
    }
}