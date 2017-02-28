using SimplyFast.Log.Internal.Outputs.Writers;
using SimplyFast.Strings.Tokens;

namespace SimplyFast.Log
{
    public static class WriterEx
    {
        public static IWriter ToLogWriter(this TokenizedWriter tokenizedWriter, IStringTokenProvider tokenProvider)
        {
            return new TokenizedLogWriter(tokenizedWriter, tokenProvider);
        }

        public static IWriter Tokenized(string tokenizedString, IStringTokenProvider tokenProvider)
        {
            var tokenizedWriter = new TokenizedWriter();
            tokenizedWriter.AddTokenizedString(tokenizedString);
            return tokenizedWriter.ToLogWriter(tokenProvider);
        }
    }
}