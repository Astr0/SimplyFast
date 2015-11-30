using System;

namespace SF.Data.Spaces
{
    public interface ILocalTransaction: IDisposable
    {
        ILocalTransaction BeginTransaction();

        void Abort();
        void Commit();
    }
}