using System;
using System.Collections.Generic;

namespace SF.Data.Spaces.Local
{
    internal class LocalSpaceProxy : ISpaceProxy
    {
        private readonly LocalSpace _space;
        private readonly LinkedList<IWaitingAction> _globalWaitingActions = new LinkedList<IWaitingAction>();

        public LocalSpaceProxy(LocalSpace space)
        {
            _space = space;
        }

        public void Dispose()
        {
            WaitingAction.RemoveAll(_globalWaitingActions);
            if (_transactionCount == 0)
                return;
            // Just rollback topmost transactions internally
            for (var i = 0; i < _transactionTablesCount; i++)
            {
                _transactionTables[i].Table.AbortToRoot();
            }
            Array.Clear(_transactionTables, 0, _transactionTablesCount);
            _transactionTablesCount = 0;
            _transactionCount = 0;
        }

        #region Transactions

        private int _transactionCount;
        public void BeginTransaction()
        {
            _transactionCount++;
            if (_transactionTables == null)
                _transactionTables = new TransactionTableHolder[LocalSpaceConsts.ProxyTableTransactionsCapacity];
        }

        public void CommitTransaction()
        {
            if (_transactionCount == 0)
                throw new InvalidOperationException("Not in transaction");
            // Find all transactions with current count
            var i = 0;
            while (i < _transactionTablesCount)
            {
                var table = _transactionTables[i].Table;
                if (table.HierarchyLevel != _transactionCount)
                {
                    ++i;
                    continue;
                }
                if (table.Parent.HierarchyLevel == 0)
                {
                    // remove this transaction
                    _transactionTablesCount--;
                    _transactionTables[i] = _transactionTables[_transactionTablesCount];
                    _transactionTables[_transactionTablesCount].Table = null;
                }
                else
                {
                    // point to parent
                    _transactionTables[i].Table = table.Parent;
                    ++i;
                }
                table.Commit();
            }
            _transactionCount--;
        }

        public void RollbackTransaction()
        {
            if (_transactionCount == 0)
                throw new InvalidOperationException("Not in transaction");
            var i = 0;
            while (i < _transactionTablesCount)
            {
                var table = _transactionTables[i].Table;
                if (table.HierarchyLevel != _transactionCount)
                {
                    ++i;
                    continue;
                }
                if (table.Parent.HierarchyLevel == 0)
                {
                    // remove this transaction
                    _transactionTablesCount--;
                    _transactionTables[i] = _transactionTables[_transactionTablesCount];
                    _transactionTables[_transactionTablesCount].Table = null;
                }
                else
                {
                    // point to parent
                    _transactionTables[i].Table = table.Parent;
                    ++i;
                }
                table.Abort();
            }
            _transactionCount--;
        }

        
        public int TransactionCount => _transactionCount;

        private TransactionTableHolder[] _transactionTables;
        private int _transactionTablesCount;

        private LocalTable<T> GetLastTransactionTable<T>(TupleType type)
        {
            if (_transactionCount == 0)
                return _space.GetRootTable<T>(type);
            // transactions have their own tables
            for (var i = 0; i < _transactionCount; i++)
            {
                if (_transactionTables[i].TupleTypeId == type.Id)
                    return (LocalTable<T>)_transactionTables[i].Table;
            }
            return _space.GetRootTable<T>(type);
        }

        private LocalTable<T> GetCurrentTransactionTable<T>(TupleType type)
        {
            if (_transactionCount == 0)
                return _space.GetRootTable<T>(type);
            // transactions have their own tables
            for (var i = 0; i < _transactionTablesCount; i++)
            {
                if (_transactionTables[i].TupleTypeId != type.Id)
                    continue;
                var table = (LocalTable<T>) _transactionTables[i].Table;
                if (table.HierarchyLevel == _transactionCount)
                    return table;
                // create nested table
                var nestedTable = LocalTable<T>.GetTransactional(_transactionCount, table);
                _transactionTables[i].Table = nestedTable;
                return nestedTable;
            }
            // nothing found
            var newNestedTable = LocalTable<T>.GetTransactional(_transactionCount, _space.GetRootTable<T>(type));
            if (_transactionTablesCount == _transactionTables.Length)
            {
                // resize _transactionTables
                Array.Resize(ref _transactionTables, _transactionTablesCount * 2);
            }
            _transactionTables[_transactionTablesCount++] =  new TransactionTableHolder(type.Id,  newNestedTable);
            return newNestedTable;
        }

        private struct TransactionTableHolder
        {
            public TransactionTableHolder(ushort tupleTypeId, ILocalTable table)
            {
                TupleTypeId = tupleTypeId;
                Table = table;
            }

            public readonly ushort TupleTypeId;
            public ILocalTable Table;
        }

        #endregion

        #region Operations

        public T TryRead<T>(IQuery<T> query)
        {
            return GetLastTransactionTable<T>(query.Type).TryRead(query);
        }

        public T TryTake<T>(IQuery<T> query)
        {
            return GetCurrentTransactionTable<T>(query.Type).TryTake(query);
        }

        public IDisposable Read<T>(IQuery<T> query, Action<T> callback)
        {
            return GetCurrentTransactionTable<T>(query.Type).Read(query, callback, _globalWaitingActions);
        }

        public IDisposable Take<T>(IQuery<T> query, Action<T> callback)
        {
            return GetCurrentTransactionTable<T>(query.Type).Take(query, callback, _globalWaitingActions);
        }

        public IReadOnlyList<T> Scan<T>(IQuery<T> query)
        {
            return GetLastTransactionTable<T>(query.Type).Scan(query);
        }

        public int Count<T>(IQuery<T> query)
        {
            return GetLastTransactionTable<T>(query.Type).Count(query);
        }

        public void Add<T>(TupleType type, T tuple)
        {
            GetCurrentTransactionTable<T>(type).Add(tuple);
        }

        public void AddRange<T>(TupleType type, T[] tuples)
        {
            GetCurrentTransactionTable<T>(type).AddRange(tuples);
        }

        

        #endregion
    }
}