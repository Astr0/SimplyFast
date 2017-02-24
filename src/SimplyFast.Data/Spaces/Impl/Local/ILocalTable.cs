namespace SimplyFast.Data.Spaces.Impl.Local
{
    public interface ILocalTable
    {
        ILocalTable Parent { get; }
        int HierarchyLevel { get; }
        void Commit();
        void Abort();
        void AbortToRoot();
    }
}