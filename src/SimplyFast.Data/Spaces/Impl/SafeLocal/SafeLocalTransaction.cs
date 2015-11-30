using System.Threading.Tasks;

namespace SF.Data.Spaces
{
    public class SafeLocalTransaction<T>: ITransaction
    {
        public async void Dispose()
        {
            await Abort();
        }

        public Task Abort()
        {
            throw new System.NotImplementedException();
        }

        public Task Commit()
        {
            throw new System.NotImplementedException();
        }
    }
}