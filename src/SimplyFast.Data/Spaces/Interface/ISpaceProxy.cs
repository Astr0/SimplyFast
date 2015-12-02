using System;

namespace SF.Data.Spaces
{
    /// <summary>
    /// Convinient interface for space access
    /// Treat like IDbConnection
    /// </summary>
    public interface ISpaceProxy: IDisposable
    {
        /// <summary>
        /// Begin transaction. Only one transaction per proxy
        /// </summary>
        ITransaction BeginTransaction();

        /// <summary>
        /// read entity from space if exists, otherwise return default
        /// </summary>
        T TryRead<T>(IQuery<T> query, ITransaction transaction = null);
        /// <summary>
        /// read and remove entity from space if exists, otherwise return default
        /// </summary>
        T TryTake<T>(IQuery<T> query, ITransaction transaction = null);
        /// <summary>
        /// read entity from space if exists, othewise wait for entity or return.dispose
        /// </summary>
        /// TODO
        IDisposable Read<T>(IQuery<T> query, Action<T> callback, ITransaction transaction = null);
        /// <summary>
        /// read entity from space if exists, othewise wait for entity or return.dispose
        /// </summary>
        /// TODO
        IDisposable Take<T>(IQuery<T> query, Action<T> callback, ITransaction transaction = null);
        /// <summary>
        /// read all matching entities
        /// </summary>
        T[] Scan<T>(IQuery<T> query, ITransaction transaction = null);
        /// <summary>
        /// count all matching entities
        /// </summary>
        int Count<T>(IQuery<T> query, ITransaction transaction = null);
        /// <summary>
        /// add new entity
        /// </summary>
        void Add<T>(T tuple, ITransaction transaction = null);
        /// <summary>
        /// add bunch of new entities
        /// </summary>
        void AddRange<T>(T[] tuples, ITransaction transaction = null);
    }
}