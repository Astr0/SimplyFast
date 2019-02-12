using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.Serialization
{
    public enum WireType : uint
    {
        [SuppressMessage("ReSharper", "IdentifierTypo")] 
        Varint,
        Fixed64,
        LengthDelimited,
        StartGroup,
        EndGroup,
        Fixed32,
    }
}