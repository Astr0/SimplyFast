using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.Serialization.interfaces
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class WireFormat
    {
        private const int WireTypeBits = 3;
        private const uint WireTypeMask = 7;

        public static WireType GetWireType(uint tag)
        {
            return (WireType) (tag & WireTypeMask);
        }

        public static int GetFieldNumber(uint tag)
        {
            return (int) tag >> WireTypeBits;
        }

        public static uint MakeTag(int fieldNumber, WireType wireType)
        {
            return (uint) (fieldNumber << WireTypeBits) | (uint) wireType;
        }
    }
}