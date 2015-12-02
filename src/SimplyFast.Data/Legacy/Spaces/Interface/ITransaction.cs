using System.Threading.Tasks;

namespace SF.Data.Legacy.Spaces
{
    public interface ITransaction : ISyncTransaction
    {
        new Task<ITransaction> BeginTransaction();
        new Task Abort();
        new Task Commit();
    }
}