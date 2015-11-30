using System;

namespace SF.Data.Spaces
{
    internal class LocalSpace : ISyncSpace
    {
        private int _nextTransactionId;
        private object[] _spaces = new object[4];

        public ISyncTransaction BeginTransaction()
        {
            return new LocalTransaction(this, null);
        }

        public ISyncSpaceTable<T> GetTable<T>(ushort id) where T : class
        {
            if (id >= _spaces.Length)
                Array.Resize(ref _spaces, id + 1);
            var result = _spaces[id];
            if (result == null)
                _spaces[id] = result = new LocalSpaceTable<T>(this, id);

            return (ISyncSpaceTable<T>) result;
        }

        internal int GetNextTransactionId()
        {
            return _nextTransactionId++;
        }
    }
}