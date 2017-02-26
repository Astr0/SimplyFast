namespace SimplyFast.Log
{
    public interface ILogInfo
    {
        string Name { get; }
        string ToString(string format);
    }
}