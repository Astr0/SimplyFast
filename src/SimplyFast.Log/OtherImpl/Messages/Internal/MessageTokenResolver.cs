namespace SimplyFast.Log.Messages.Internal
{
    public delegate string MessageTokenResolver<in T>(T message, string format) where T : IMessage;
}