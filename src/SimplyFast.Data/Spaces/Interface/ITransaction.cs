using System;

namespace SF.Data.Spaces
{
    public interface ITransaction: IDisposable
    {
        TransactionState State { get; }
        ITransaction BeginTransaction();
        void Commit();
        void Rollback();
    }
}