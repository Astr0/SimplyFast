namespace SF.Data.Dynamo
{
    public static class DObjectEx
    {
        public static bool GetBool(this DObject obj, int property, bool def)
        {
            return obj.GetByte(property, def ? (byte)1 : (byte)0) != 0;
        }

        public static sbyte GetSByte(this DObject obj, int property, sbyte def)
        {
            return (sbyte)obj.GetByte(property, (byte)def);
        }

        public static short GetShort(this DObject obj, int property, short def)
        {
            return (short)obj.GetUShort(property, (ushort)def);
        }

        public static int GetInt(this DObject obj, int property, int def)
        {
            return (int)obj.GetUInt(property, (uint)def);
        }

        public static long GetLong(this DObject obj, int property, long def)
        {
            return (long)obj.GetULong(property, (ulong)def);
        }

        public static void SetBool(this DObject obj, int property, bool value)
        {
            obj.SetByte(property, value ? (byte)1 : (byte)0);
        }

        public static void SetSByte(this DObject obj, int property, sbyte value)
        {
            obj.SetByte(property, (byte)value);
        }

        public static void SetShort(this DObject obj, int property, short value)
        {
            obj.SetUShort(property, (ushort)value);
        }

        public static void SetInt(this DObject obj, int property, int value)
        {
            obj.SetUInt(property, (uint)value);
        }

        public static void SetLong(this DObject obj, int property, long value)
        {
            obj.SetULong(property, (ulong)value);
        }
    }
}