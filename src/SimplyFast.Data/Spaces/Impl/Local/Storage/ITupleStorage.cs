using System;
using SimplyFast.Data.Spaces.Interface;

namespace SimplyFast.Data.Spaces.Impl.Local.Storage
{
    public interface ITupleStorage<T>
    {
        void Add(T tuple);
        T Read(IQuery<T> query);
        T Take(IQuery<T> query);
        void Scan(IQuery<T> query, Action<T> callback);
        int Count(IQuery<T> query);
        void Clear();
        T[] GetArray(out int count);
        void AddRange(T[] tuples, int count);
    }
}