using System;
using System.Collections.Generic;

namespace SF.Data.Legacy.Spaces
{
    internal class LocalSpace : ISyncSpace
    {
        private ILocalSpaceTable[] _tables = new ILocalSpaceTable[4];
        
        public ISyncSpaceTable<T> GetTable<T>(ushort id) where T : class
        {
            if (id >= _tables.Length)
                Array.Resize(ref _tables, id + 1);
            var result = _tables[id];
            if (result == null)
            {
                _tables[id] = result = new LocalSpaceTable<T>(this, id, _transactionsCapacity);
            }

            return (ISyncSpaceTable<T>) result;
        }

        private int _nextTransactionId;
        private int _transactionsCapacity = Math.Max(1, LocalSpaceConsts.TransactionsCapacity);
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
            if (_freeTransactionIds.Count != 0)
                return _freeTransactionIds.Pop();
            var id = _nextTransactionId++;
            if (_nextTransactionId < _transactionsCapacity)
                return id;
            // ensure capactiy for spaces
            _transactionsCapacity = _transactionsCapacity*2;
            foreach (var table in _tables)
            {
                table?.EnsureTransactionsCapacity(_nextTransactionId);
            }
            return id;
        }
    }
}