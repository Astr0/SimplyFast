using System.IO;

namespace SimplyFast.Log.Internal.Outputs.Writers
{
    public static class InfoWriterEx
    {
        public static string GetString(this InfoWriter writer, ILogInfoProvider provider)
        {
            using (var sw = new StringWriter())
            {
                writer.Write(sw, provider);
                return sw.ToString();
            }
        }
    }
}