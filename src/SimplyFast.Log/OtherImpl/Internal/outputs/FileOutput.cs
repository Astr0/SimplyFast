using System.IO;
using System.Text;

namespace SimplyFast.Log.Internal
{
    internal class FileOutput : TextWriterOutput
    {
        public FileOutput(string fileName, IWriter format, bool append = true)
            : base(CreateFileStreamWriter(fileName, append), format)
        {
        }

        private static StreamWriter CreateFileStreamWriter(string fileName, bool append)
        {
            var dir = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            var fileMode = append ? FileMode.Append : FileMode.Create;
            var stream = File.Open(fileName, fileMode, FileAccess.Write, FileShare.Read);
            var writer = new StreamWriter(stream, Encoding.UTF8);
            return writer;
        }
    }
}