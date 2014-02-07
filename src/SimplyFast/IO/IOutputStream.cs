using System;
using System.Threading.Tasks;

namespace SF.IO
{
    public interface IOutputStream : IDisposable
    {
        /// <summary>
        /// Writes count of bytes from buffer starting at offset to stream.
        /// </summary>
        /// <returns>Task to check if write completed</returns>
        Task Write(byte[] buffer, int offset, int count);
    }
}