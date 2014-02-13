using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;

namespace SF.Pipes
{
    internal class DbDataReaderConsumer : IConsumer<IDataRecord>
    {
        private readonly DbDataReader _reader;

        public DbDataReaderConsumer(DbDataReader reader)
        {
            _reader = reader;
        }

        #region IConsumer<IDataRecord> Members

        public void Dispose()
        {
            _reader.Dispose();
        }

        public async Task<IDataRecord> Take()
        {
            var read = await _reader.ReadAsync();
            if (!read)
                throw new EndOfStreamException();
            return _reader;
        }

        #endregion
    }
}