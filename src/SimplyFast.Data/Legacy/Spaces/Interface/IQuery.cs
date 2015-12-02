namespace SF.Data.Legacy.Spaces
{
    public interface IQuery<in T>
    {
        bool Match(T tuple);
    }
}