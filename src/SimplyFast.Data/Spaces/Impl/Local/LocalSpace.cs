using System;
using System.Collections.Generic;

namespace SF.Data.Spaces
{
    internal class LocalSpace : ISyncSpace
    {
        private object[] _tables = new object[4];
        
        public ISyncSpaceTable<T> GetTable<T>(ushort id) where T : class
        {
            if (id >= _tables.Length)
                Array.Resize(ref _tables, id + 1);
            var result = _tables[id];
            if (result == null)
                _tables[id] = result = new LocalSpaceTable<T>(this, id);

            return (ISyncSpaceTable<T>) result;
        }

        private int _nextTransactionId;
        private readonly Stack<int> _freeTransactionIds = new Stack<int>(LocalSpaceConsts.TransactionsCapacity);
        public ISyncTransaction BeginTransaction()
        {
            return new RootLocalTransaction(this);
        }

        internal void Cleanup(int transactionId)
        {
            _freeTransactionIds.Push(transactionId);
        }

        internal int GetNextTransactionId()
        {
            return _freeTransactionIds.Count != 0 ? _freeTransactionIds.Pop() : _nextTransactionId++;
        }
    }
}