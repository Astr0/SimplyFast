using System;
using System.IO;

namespace SimplyFast.Strings.Tokens
{
    public static class TokenWriterEx
    {
        public static void Write(this TokenizedWriter tokenizedWriter, TextWriter writer, IStringTokenProvider provider)
        {
            tokenizedWriter.Write(writer, provider.Get);
        }

        public static string GetString(this TokenizedWriter writer, Func<string, IStringToken> resolver)
        {
            using (var sw = new StringWriter())
            {
                writer.Write(sw, resolver);
                return sw.ToString();
            }
        }

        public static string GetString(this TokenizedWriter writer, IStringTokenProvider provider)
        {
            return writer.GetString(provider.Get);
        }
    }
}