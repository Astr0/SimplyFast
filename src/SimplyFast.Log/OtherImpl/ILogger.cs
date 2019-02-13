using SimplyFast.Log.Messages;

namespace SimplyFast.Log
{
    public interface ILogger
    {
        Severity Severity { get; set; }
        string Name { get; }
        IOutputs Outputs { get; }
        void Log(IMessage message);
    }
}