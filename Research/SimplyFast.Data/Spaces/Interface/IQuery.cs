namespace SF.Data.Spaces
{
    public interface IQuery<in T>
    {
        TupleType Type { get; }
        bool Match(T tuple);
    }
}