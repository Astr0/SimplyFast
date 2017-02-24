namespace SimplyFast.IO
{
    public static class VarIntHelper
    {
        public const int VarIntDataBits = 7;
        public const byte VarIntDataMask = 0x7F;
        public const byte VarIntMoreFlag = 0x80;

        
        public static int GetVarInt32Size(uint value)
        {
            const uint dataMask = VarIntDataMask;
            const uint moreThan1 = ~dataMask;
            const uint moreThan2 = moreThan1 << VarIntDataBits;
            const uint moreThan3 = moreThan2 << VarIntDataBits;
            const uint moreThan4 = moreThan3 << VarIntDataBits;
            
            if ((value & moreThan1) == 0)
                return 1;
            if ((value & moreThan2) == 0)
                return 2;
            if ((value & moreThan3) == 0)
                return 3;
            return (value & moreThan4) == 0 ? 4 : 5;
        }

        public static int GetVarInt64Size(ulong value)
        {
            const ulong dataMask = VarIntDataMask;
            const ulong moreThan1 = ~dataMask;
            const ulong moreThan2 = moreThan1 << VarIntDataBits;
            const ulong moreThan3 = moreThan2 << VarIntDataBits;
            const ulong moreThan4 = moreThan3 << VarIntDataBits;
            const ulong moreThan5 = moreThan4 << VarIntDataBits;
            const ulong moreThan6 = moreThan5 << VarIntDataBits;
            const ulong moreThan7 = moreThan6 << VarIntDataBits;
            const ulong moreThan8 = moreThan7 << VarIntDataBits;
            const ulong moreThan9 = moreThan8 << VarIntDataBits;


            if ((value & moreThan1) == 0)
                return 1;
            if ((value & moreThan2) == 0)
                return 2;
            if ((value & moreThan3) == 0)
                return 3;
            if ((value & moreThan4) == 0)
                return 4;
            if ((value & moreThan5) == 0)
                return 5;
            if ((value & moreThan6) == 0)
                return 6;
            if ((value & moreThan7) == 0)
                return 7;
            if ((value & moreThan8) == 0)
                return 8;
            return (value & moreThan9) == 0 ? 9 : 10;
        }

        public static byte[] GetVarInt32Bytes(uint value)
        {
            var size = GetVarInt32Size(value);
            var result = new byte[size];
            var writer = new FastBufferWriter(result);
            writer.WriteVarInt32(value);
            return result;
        }

        public static byte[] GetVarInt64Bytes(ulong value)
        {
            var size = GetVarInt64Size(value);
            var result = new byte[size];
            var writer = new FastBufferWriter(result);
            writer.WriteVarInt64(value);
            return result;
        }
    }
}