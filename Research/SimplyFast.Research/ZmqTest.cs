using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SF.Collections;
using SF.Net.Sockets;
using SF.Pipes;

namespace SimplyFast.Research
{
    public class ZmqTest: ResearchBase
    {

        private const int Clients = 300;

        public static async void Work()
        {
            var factory = new ZmqSocketFactory();

            const string endPoint = "tcp://127.0.0.1:3232";
            var tasks = new List<Task>
            {
                StartServer(factory, endPoint)
            };
            for (var i = 0; i < Clients; ++i)
            {
                tasks.Add(StartClient(i, factory, endPoint));
            }

            await Task.WhenAll(tasks);
        }

        private static async Task StartClient(int clientId, ZmqSocketFactory factory, string address)
        {
            using (var client = factory.CreateDealer())
            {
                client.Connect(address);
                for (var i = 0; i < 100; i++)
                {
                    await client.Add(GenerateBuffer(i));
                    DebugWrite("Send " + i + " from " + clientId);
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

        private static async Task StartServer(ZmqSocketFactory factory, string address)
        {
            using (var server = factory.CreateRouter())
            {
                server.Bind(address);
                var consumer = server as IConsumer<IReadOnlyList<byte[]>>;
                var i = new Dictionary<string, int>();
                while (true)
                {
                    try
                    {
                        var input = await consumer.Take();
                        var who = BitConverter.ToString(input[0].ToArray());
                        var read = input[1];
                        var equal = read.SequenceEqual(GenerateBuffer(i.GetOrAdd(who, x => 0)));
                        DebugWrite(read.Length + " bytes received. From " + who + ". Equal " + equal);
                        i[who] = i[who] + 1;
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