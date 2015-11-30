using System;
using System.Collections.Generic;

namespace SF.Data.Spaces
{
    public interface ILocalSpace<T>: IDisposable
        where T: class
    {
        ILocalTransaction BeginTransaction();
        // Read or nothing
        T TryRead(IQuery<T> query, ILocalTransaction transaction = null);
        // Take or nothing
        T TryTake(IQuery<T> query, ILocalTransaction transaction = null);

        // Read or wait
        IDisposable Read(IQuery<T> query, Action<T> callback, ILocalTransaction transaction = null);
        // Take or wait
        IDisposable Take(IQuery<T> query, Action<T> callback, ILocalTransaction transaction = null);

        T[] Scan(IQuery<T> query, ILocalTransaction transaction = null);
        int Count(IQuery<T> query, ILocalTransaction transaction = null);
        void Add(T tuple, ILocalTransaction transaction = null);
        void AddRange(IEnumerable<T> tuple, ILocalTransaction transaction = null); 
    }
}