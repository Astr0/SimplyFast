namespace SimplyFast.Serialization.interfaces
{
    public enum WireType : uint
    {
        Varint,
        Fixed64,
        LengthDelimited,
        StartGroup,
        EndGroup,
        Fixed32,
    }
}