namespace SimplyFast.Serialization
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