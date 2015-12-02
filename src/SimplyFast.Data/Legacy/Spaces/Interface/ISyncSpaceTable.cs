using System;

namespace SF.Data.Legacy.Spaces
{
    public interface ISyncSpaceTable<T> : IDisposable
        where T : class
    {
        // Read or nothing
        T TryRead(IQuery<T> query, ISyncTransaction transaction = null);
        // Take or nothing
        T TryTake(IQuery<T> query, ISyncTransaction transaction = null);
        // Read or wait
        void Read(IQuery<T> query, Action<T> callback, TimeSpan timeout, ISyncTransaction transaction = null);
        // Take or wait
        void Take(IQuery<T> query, Action<T> callback, TimeSpan timeout, ISyncTransaction transaction = null);
        T[] Scan(IQuery<T> query, ISyncTransaction transaction = null);
        int Count(IQuery<T> query, ISyncTransaction transaction = null);
        void Add(T tuple, ISyncTransaction transaction = null);
        void AddRange(T[] tuples, ISyncTransaction transaction = null);
    }
}