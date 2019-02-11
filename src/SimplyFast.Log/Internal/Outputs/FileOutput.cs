
using System.IO;
using System.Text;

namespace SimplyFast.Log.Internal.Outputs
{
    internal class FileOutput : TextWriterOutput
    {
        private static StreamWriter CreateFileStreamWriter(string fileName, bool append)
        {
            var dir = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(dir))
            {
                // ensure directory exists
                Directory.CreateDirectory(dir);
            }
            var fileMode = append ? FileMode.Append : FileMode.Create;
            var stream = File.Open(fileName, fileMode, FileAccess.Write, FileShare.Read);
            var writer = new StreamWriter(stream, Encoding.UTF8);
            return writer;
        }

        public FileOutput(string fileName, IWriter writer, bool append = true)
            : base(CreateFileStreamWriter(fileName, append), writer)
        {
        }
    }
}