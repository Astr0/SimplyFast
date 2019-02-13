using System.IO;
using SimplyFast.Strings.Tokens;

namespace SimplyFast.Log.Internal.Outputs.Writers
{
    internal class TokenizedLogWriter : IWriter
    {
        private readonly IStringTokenProvider _tokenProvider;
        private readonly TokenizedWriter _writer;

        public TokenizedLogWriter(TokenizedWriter writer, IStringTokenProvider tokenProvider)
        {
            _writer = writer;
            _tokenProvider = tokenProvider;
        }

        public void Write(TextWriter writer, IMessage message)
        {
            _writer.Write(writer, name => message.Get(name) ?? _tokenProvider.Get(name));
        }
    }
}