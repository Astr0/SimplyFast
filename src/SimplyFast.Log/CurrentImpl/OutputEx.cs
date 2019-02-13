using System.IO;
using SimplyFast.Log.Internal.Outputs;

namespace SimplyFast.Log
{
    public static class OutputEx
    {
        public static IOutputs DefaultOutputs()
        {
            return new DefaultOutputs();
        }

        public static IOutput TextWriter(TextWriter textWriter, IWriter writer, bool leaveOpen = false)
        {
            return new TextWriterOutput(textWriter, writer, leaveOpen);
        }

        public static IOutput OutputSeverity(this IOutput output, Severity severity)
        {
            return new SeverityOutput(severity, output);
        }

        public static IOutput Console(IWriter writer)
        {
            return new ConsoleOutput(writer);
        }
        public static IOutput File(string fileName, IWriter writer, bool append = true)
        {
            return new FileOutput(fileName, writer, append);
        }
    }
}