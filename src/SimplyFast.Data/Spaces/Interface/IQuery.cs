namespace SimplyFast.Data.Spaces.Interface
{
    public interface IQuery<in T>
    {
        TupleType Type { get; }
        bool Match(T tuple);
    }
}