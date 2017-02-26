#if FILES
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
// ReSharper disable once RedundantUsingDirective
using SimplyFast.IO;

namespace SimplyFast.Tests.IO
{
    public class FileExTests
    {
        private static byte[] GetTestBytes()
        {
            var bytes = new byte[1000000];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(i % 256);
            }
            return bytes;
        }

        [Fact]
        public async Task ReadAllBytesOk()
        {
            var bytes = GetTestBytes();
            var tmp = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tmp, bytes);
                var read = await FileEx.ReadAllBytes(tmp);
                Assert.True(read.SequenceEqual(bytes));
            }
            finally
            {
                if (File.Exists(tmp))
                    File.Delete(tmp);
            }
        }

        [Fact]
        public async Task WriteAllBytesOk()
        {
            var bytes = GetTestBytes();
            var tmp = Path.GetTempFileName();
            try
            {
                await FileEx.WriteAllBytes(tmp, bytes);
                var read = File.ReadAllBytes(tmp);
                Assert.True(read.SequenceEqual(bytes));
            }
            finally
            {
                if (File.Exists(tmp))
                    File.Delete(tmp);
            }
        }
    }
}
#endif