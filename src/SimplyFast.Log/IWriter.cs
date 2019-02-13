using System.IO;
using SimplyFast.Log.Messages;

namespace SimplyFast.Log
{
    public interface IWriter
    {
        void Write(TextWriter writer, IMessage message);
    }
}