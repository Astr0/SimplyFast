namespace SimplyFast.Log
{
    public interface ILoggerFactory
    {
        ILogger Root { get; }
        ILogger Get(object key);
    }
}