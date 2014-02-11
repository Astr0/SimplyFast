using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SF.Network.Sockets;
using SF.Pipes;
using SF.Threading;

namespace SimplyFast.Research
{
    internal class Program
    {
        public static void DebugWrite(string caption)
        {
            Console.WriteLine(caption + " " + Thread.CurrentThread.ManagedThreadId + " " + TaskScheduler.Current.Id);
        }

        
        private static async void Work()
        {
            var factory = new NetFactory();

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

        private static async Task StartClient(NetFactory factory, EndPoint endPoint)
        {
            using (var client = await factory.Connect(endPoint))
            {
                var consumer = client.AsVarIntLengthPrefixedProducer();
                for (var i = 0; i < 100; i++)
                {
                    await consumer.Add(new ArraySegment<byte>(GenerateBuffer(i)));
                }
                await client.Disconnect();
            }
        }

        private static byte[] GenerateBuffer(int i)
        {
            var res = new byte[i*100];
            for (var j = 0; j < res.Length; j++)
            {
                res[j] = (byte) ((i + j)%255);
            }
            return res;
        }

        private static async Task StartServer(NetFactory factory, EndPoint endPoint)
        {
            var server = factory.Listen(endPoint);
            while (true)
            {
                var client = await server.Accept();
                StartClientTask(client);
            }
        }

        private static async void StartClientTask(NetSocket client)
        {
            using (var consumer = client.AsVarIntLengthPrefixedConsumer())
            {
                var i = 0;
                while (true)
                {
                    try
                    {
                        var read = await consumer.Take();
                        var equal = read.SequenceEqual(GenerateBuffer(i));
                        i++;
                        DebugWrite(read.Count + " bytes received. Equal " + equal);
                    }
                    catch (EndOfStreamException)
                    {
                        DebugWrite("Client disconnected");
                        return;
                    }
                }
            }
        }

        

        private static void Main()
        {
            //Work();
            EventLoop.Run(Work);
            Console.ReadLine();
        }
    }
}