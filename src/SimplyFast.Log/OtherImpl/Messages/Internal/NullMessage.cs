namespace SimplyFast.Log.Messages.Internal
{
    public class NullMessage : IMessage
    {
        public static readonly IMessage Instance = new NullMessage();

        protected NullMessage()
        {
        }

        public virtual string Get(MessageToken token, string format)
        {
            return MessageTokens.GetTokenValue(token, format);
        }

        public Severity Severity => Severity.Off;
    }
}