namespace SimplyFast.Configuration
{
    public interface IReadOnlyConfig
    {
        string this[string key] { get; }
    }
}