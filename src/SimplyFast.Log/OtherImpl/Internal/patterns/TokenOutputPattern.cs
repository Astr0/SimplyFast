using SimplyFast.Log.Messages;
using SimplyFast.Strings;

namespace SimplyFast.Log.Internal
{
    public class TokenOutputPattern : IOutputPattern
    {
        private readonly string _format;
        private readonly MessageToken _token;

        public TokenOutputPattern(MessageToken token, string format)
        {
            _token = token;
            _format = format;
        }

        public string GetValue(IMessage message)
        {
            return message.Get(_token, _format);
        }

        public static IOutputPattern ParseToken(string str)
        {
            var sp = new StringParser(str);
            var token = sp.SubstringTo(':');
            if (string.IsNullOrEmpty(token))
                return null;
            sp.Skip(1);
            var format = sp.Right;
            if (string.IsNullOrEmpty(format))
                format = null;
            return new TokenOutputPattern(MessageTokens.Get(token), format);
        }

        public override string ToString()
        {
            return "%" + _token + (_format != null ? ":" + _format : "") + "%";
        }
    }
}