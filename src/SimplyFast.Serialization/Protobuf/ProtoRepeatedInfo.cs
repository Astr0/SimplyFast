namespace SimplyFast.Serialization.Protobuf
{
    internal static class ProtoRepeatedInfo<T>
    {
        private static readonly bool TypeCanBePacked = default (T) != null;

        internal static bool IsPackedRepeatedField(uint tag)
        {
            if (TypeCanBePacked)
                return WireFormat.GetWireType(tag) == WireType.LengthDelimited;
            return false;
        }
    }
}