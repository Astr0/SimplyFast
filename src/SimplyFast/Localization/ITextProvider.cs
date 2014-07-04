namespace SF.Localization
{
    public interface ITextProvider
    {
        string this[string key] { get; }
    }
}