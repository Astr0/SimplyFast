namespace SimplyFast.Configuration
{
    public interface IConfig : IReadOnlyConfig
    {
        new string this[string key] { get; set; }
    }
}