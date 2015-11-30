namespace SF.Data.Spaces
{
    public interface ISyncSpace
    {
        ISyncTransaction BeginTransaction();
        ISyncSpaceTable<T> GetTable<T>(ushort id) where T : class;
    }
}