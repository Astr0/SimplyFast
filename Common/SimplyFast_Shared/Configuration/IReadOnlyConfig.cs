namespace SF.Configuration
{
    public interface IReadOnlyConfig
    {
        string this[string key] { get; }
    }
}