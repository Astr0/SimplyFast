namespace SF.IO
{
    public static class BufferWriter
    {
        /// <summary>
        /// Write int as VarInt to buffer, returns bytes written
        /// </summary>
        public static int WriteVarInt32(byte[] buffer, int offset, int value)
        {
            if (value >= 0)
                return WriteVarUInt32(buffer, offset, (uint)value);
            buffer[offset] = (byte)(value | 128);
            buffer[offset + 1] = (byte)((value >> 7) | 128);
            buffer[offset + 2] = (byte)((value >> 14) | 128);
            buffer[offset + 3] = (byte)((value >> 21) | 128);
            buffer[offset + 4] = (byte)((value >> 28) | 128);
            buffer[offset + 5] = 0xff;
            buffer[offset + 6] = 0xff;
            buffer[offset + 7] = 0xff;
            buffer[offset + 8] = 0xff;
            buffer[offset + 9] = 1;
            return 10;
        }

        /// <summary>
        /// Write uint as VarInt to buffer, returns bytes written
        /// </summary>
        public static int WriteVarUInt32(byte[] buffer, int offset, uint value)
        {
            var index = 0;
            while (value >= 128U)
            {
                buffer[offset + index] = (byte)(value | 128U);
                value >>= 7;
                index++;
            }
            buffer[offset + index] = (byte)value;
            return index + 1;
        }
    }
}