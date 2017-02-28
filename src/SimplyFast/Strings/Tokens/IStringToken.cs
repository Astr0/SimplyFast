namespace SimplyFast.Strings.Tokens
{
    public interface IStringToken
    {
        string Name { get; }
        string ToString(string format);
    }
}