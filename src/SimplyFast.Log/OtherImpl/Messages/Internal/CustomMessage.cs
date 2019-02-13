using System;

namespace SimplyFast.Log.Messages.Internal
{
    internal class CustomMessage: IMessage
    {
        private readonly Func<IMessage, MessageToken, string, string> _get;

        public CustomMessage(Severity severity, Func<IMessage, MessageToken, string, string> get)
        {
            _get = get;
            Severity = severity;
        }

        public Severity Severity { get; }

        public string Get(MessageToken token, string format = null)
        {
            return _get?.Invoke(this, token, format);
        }
    }
}