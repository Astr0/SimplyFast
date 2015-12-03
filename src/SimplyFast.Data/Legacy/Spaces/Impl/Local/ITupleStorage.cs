﻿using System;

namespace SF.Data.Legacy.Spaces
{
    public interface ITupleStorage<T>
        where T:class
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