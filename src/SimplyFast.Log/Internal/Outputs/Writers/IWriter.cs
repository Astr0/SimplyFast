using System.IO;

namespace SimplyFast.Log.Internal.Outputs.Writers
{
    public interface IWriter
    {
        void Write(TextWriter writer, IMessage message);
    }
}