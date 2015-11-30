using System;

namespace SF.Data.Spaces
{
    public interface ISyncTransaction : IDisposable
    {
        /// <summary>
        ///     Get state of current transaction
        /// </summary>
        TransactionState State { get; }

        /// <summary>
        ///     Begin nested transaction
        /// </summary>
        ISyncTransaction BeginTransaction();

        /// <summary>
        ///     Abort transaction
        /// </summary>
        void Abort();

        /// <summary>
        ///     Commit transaction
        /// </summary>
        void Commit();
    }
}