using System.IO;

namespace SimplyFast.Log
{
    public interface IWriter
    {
        void Write(TextWriter writer, IMessage message);
    }
}