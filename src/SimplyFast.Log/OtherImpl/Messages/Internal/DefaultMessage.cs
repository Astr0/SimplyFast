using System;

namespace SimplyFast.Log.Messages.Internal
{
    internal class DefaultMessage : IMessage
    {
        private static readonly MessageTokenResolvers<DefaultMessage> _resolvers = new MessageTokenResolvers
            <DefaultMessage>
            {
                {MessageTokens.Message, (m, f) => m.Message},
                {MessageTokens.Logger, (m, f) => m._source.Name},
                {MessageTokens.Date, (m, f) => m._date.ToString(f)},
                {MessageTokens.Severity, (m, f) => m.Severity.ToStr(f)}
            };

        private readonly DateTime _date;

        private readonly IMessageFactory _factory;
        private readonly Func<string> _getMessage;
        private readonly ILogger _source;
        private string _message;

        public DefaultMessage(IMessageFactory factory, ILogger source, Severity severity,
            Func<string> getMessage)
        {
            _date = DateTime.Now;
            _factory = factory;
            _source = source;
            Severity = severity;
            _getMessage = getMessage;
        }

        private string Message => _message ?? (_message = _getMessage());

        public Severity Severity { get; }

        public string Get(MessageToken token, string format)
        {
            return _resolvers.GetTokenValue(this, token, format) ??
                   _factory.Get(this, token, format);
            //MessageTokens.GetTokenValue(token, format);
        }
    }
}