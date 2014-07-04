using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Pipes
{
    internal class VarIntLengthPrefixedStreamConsumer : IConsumer<ArraySegment<byte>>
    {
        private readonly Stream _stream;
        private byte[] _buffer;
        private int _end;
        private int _offset;

        public VarIntLengthPrefixedStreamConsumer(Stream stream, int bufferCapacity)
        {
            _stream = stream;
            _buffer = new byte[Math.Min(5, bufferCapacity)];
        }

        #region IConsumer<ArraySegment<byte>> Members

        public async Task<ArraySegment<byte>> Take(CancellationToken cancellation)
        {
            // Read length
            var length = (int)await ReadLength(cancellation);

            // Fill count bytes to buffer
            if (_end - _offset < length)
                await FillBuffer(length, cancellation);

            // prepare result
            var result = new ArraySegment<byte>(_buffer, _offset, length);
            // shift buffer offset
            _offset += length;

            return result;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        #endregion

        private async Task FillBuffer(CancellationToken cancellation)
        {
            if (_offset == _end)
            {
                // Get free space in buffer
                if (_offset == 0)
                {
                    // _end remains the same
                    Array.Resize(ref _buffer, _buffer.Length*2);
                }
                else
                {
                    // copy data to the start of buffer
                    Array.Copy(_buffer, _offset, _buffer, 0, _end - _offset);
                    // shift end to offset
                    _end -= _offset;
                    // new offset is zero
                    _offset = 0;
                }
            }
            // Attemp to fill the buffer
            var read = await _stream.ReadAsync(_buffer, _end, _buffer.Length - _end, cancellation);
            if (read == 0)
                throw new EndOfStreamException();
            // shift end to bytes read
            _end += read;
        }

        private async Task FillBuffer(int totalCount, CancellationToken cancellation)
        {
            // fill more bytes to buffer, to contain at least totalCount bytes
            if (_buffer.Length - _offset < totalCount)
            {
                // Need free space in buffer
                if (_buffer.Length >= totalCount)
                {
                    // We can move buffer data
                    Array.Copy(_buffer, _offset, _buffer, 0, _end - _offset);
                }
                else
                {
                    // Need to allocate new buffer
                    var newBuffer = new byte[totalCount];
                    Array.Copy(_buffer, _offset, newBuffer, 0, _end - _offset);
                    _buffer = newBuffer;
                }
                _end -= _offset;
                _offset = 0;
            }
            // Attemp to fill the buffer
            while (_end - _offset < totalCount)
            {
                var read = await _stream.ReadAsync(_buffer, _end, _buffer.Length - _end, cancellation);
                if (read == 0)
                    throw new EndOfStreamException();
                // shift end to bytes read
                _end += read;
            }
        }

        private async Task<uint> ReadLength(CancellationToken cancellation)
        {
            // 1
            if (_offset == _end)
                await FillBuffer(cancellation);
            uint value = _buffer[_offset++];
            if ((value & 128) == 0)
                return value;
            value &= 127;

            // 2
            if (_offset == _end)
                await FillBuffer(cancellation);
            uint readByte = _buffer[_offset++];
            value |= (readByte & 127) << 7;
            if ((readByte & 128) == 0)
                return value;

            // 3
            if (_offset == _end)
                await FillBuffer(cancellation);
            readByte = _buffer[_offset++];
            value |= (readByte & 127) << 14;
            if ((readByte & 128) == 0)
                return value;

            // 4
            if (_offset == _end)
                await FillBuffer(cancellation);
            readByte = _buffer[_offset++];
            value |= (readByte & 127) << 21;
            if ((readByte & 128) == 0)
                return value;

            //5
            if (_offset == _end)
                await FillBuffer(cancellation);
            readByte = _buffer[_offset++];
            value |= readByte << 28;
            // magic number stuff copied from ProtoBuf.Net
            if ((readByte & 240) == 0)
                return value;

            throw new OverflowException("Invalid 7bit encoded UInt32");
        }
    }
}