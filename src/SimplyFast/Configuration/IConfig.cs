namespace SF.Configuration
{
    public interface IConfig : IReadOnlyConfig
    {
        new string this[string key] { get; set; }
    }
}