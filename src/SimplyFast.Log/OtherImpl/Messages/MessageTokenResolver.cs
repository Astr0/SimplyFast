namespace SimplyFast.Log.Messages
{
    public delegate string MessageTokenResolver<in T>(T message, string format) where T : IMessage;
}