namespace SF.Data.Spaces
{
    public interface IQuery<in T>
    {
        bool Match(T tuple);
    }
}