using System.IO;
using SimplyFast.Log.Internal;
using SimplyFast.Log.Messages;

namespace SimplyFast.Log
{
    // TODO: Tests
    public static class WriterEx
    {
        public static IWriter TokenString(string stringWithTokens)
        {
            return TokenStringWriter.Parse(stringWithTokens);
        }

        public static string ToString(this IWriter writer, IMessage message)
        {
            using (var sw = new StringWriter())
            {
                writer.Write(sw, message);
                return sw.ToString();
            }
        }
    }
}