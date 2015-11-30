using System.Threading.Tasks;

namespace SF.Data.Spaces
{
    public interface ISpace : ISyncSpace
    {
        new Task<ITransaction> BeginTransaction();
        new Task<ISpaceTable<T>> GetTable<T>(ushort id) where T : class;
    }
}