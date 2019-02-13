namespace SimplyFast.Log.Messages
{
    public interface IMessage
    {
        Severity Severity { get; }
        string Get(MessageToken token, string format = null);
    }
}