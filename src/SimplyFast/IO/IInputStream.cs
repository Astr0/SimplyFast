using System;
using System.Threading.Tasks;

namespace SF.IO
{
    public interface IInputStream: IDisposable
    {
        /// <summary>
        /// Reads up to count bytes from stream to buffer starting at offset
        /// </summary>
        /// <returns>Task with number of bytes actually read</returns>
        Task<int> Read(byte[] buffer, int offset, int count);
    }
}