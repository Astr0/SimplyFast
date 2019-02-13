using SimplyFast.Log.Messages;

namespace SimplyFast.Log.Internal
{
    public interface IOutputPattern
    {
        string GetValue(IMessage message);
    }
}