using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SF.Net.Sockets;
using SF.Pipes;

namespace SimplyFast.Research
{
    public class NetTest: ResearchBase
    {
        public static async void Work()
        {
            var factory = new NetSocketFactory();

            var endPoint = new IPEndPoint(IPAddress.Loopback, 3232);
            var tasks = new List<Task>
            {
                StartServer(factory, endPoint),
                StartClient(factory, endPoint),
                StartClient(factory, endPoint),
                StartClient(factory, endPoint),
            };

            await Task.WhenAll(tasks);
        }

        private static async Task StartClient(NetSocketFactory factory, EndPoint endPoint)
        {
            using (var client = await factory.Connect(endPoint))
            {
                var producer = client.Stream.AsIntLengthPrefixedProducer();
                for (var i = 0; i < 100; i++)
                {
                    await producer.Add(GenerateBuffer(i));
                }
                await client.Disconnect();
            }
        }

        private static byte[] GenerateBuffer(int i)
        {
            var res = new byte[i * 100];
            for (var j = 0; j < res.Length; j++)
            {
                res[j] = (byte)((i + j) % 255);
            }
            return res;
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        private static async Task StartServer(NetSocketFactory factory, EndPoint endPoint)
        {
            var server = factory.Listen(endPoint);
            while (true)
            {
                var client = await server.Accept();
                StartClientTask(client);
            }
        }

        private static async void StartClientTask(ISocket client)
        {
            using (var consumer = client.Stream.AsIntLengthPrefixedConsumer())
            {
                var i = 0;
                while (true)
                {
                    try
                    {
                        var read = await consumer.Take();
                        var equal = read.SequenceEqual(GenerateBuffer(i));
                        i++;
                        DebugWrite(read.Length + " bytes received. Equal " + equal);
                    }
                    catch (EndOfStreamException)
                    {
                        DebugWrite("Client disconnected");
                        return;
                    }
                }
            }
        }
    }
}