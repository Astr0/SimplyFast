using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SF.IO;
using SF.Network.Sockets;
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
            var tasks = new List<Task>();
            tasks.Add(RunServer(factory));
            tasks.AddRange(Enumerable.Range(0, 100).Select(x => RunClient(factory)));
            await Task.WhenAll(tasks);
        }

        private static byte[] GenerateData(int i)
        {
            var res = new byte[i*100];
            for (int j = 0; j < res.Length; j++)
            {
                res[j] = (byte)((i + j) % 255);
            }
            return res;
        }

        private static async Task RunClient(NetFactory factory)
        {
            //var cts = new CancellationTokenSource(500);
            try
            {
                //using (var client = await factory.Connect(new IPEndPoint(IPAddress.Loopback, 3232), cts.Token))
                using (var client = await factory.Connect(new IPEndPoint(IPAddress.Loopback, 3232)))
                {
                    Console.WriteLine("Connected");
                    for (int i = 0; i < 10; i++)
                    {
                        var buf = GenerateData(i);
                        //Console.WriteLine("Sending data");
                        await client.Write(buf);
                        Console.WriteLine(buf.Length + " bytes sent.");
                    }
                    
                    await client.Disconnect();
                    //await client.Write(GenerateData(2));
                    Console.WriteLine("Disconnected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client exception " + ex);
            }
        }

        private static async Task RunServer(NetFactory factory)
        {
            //await Task.Delay(2000);
            using (var server = factory.Listen(new IPEndPoint(IPAddress.Loopback, 3232),backlog:30))
            {
                while (true)
                {
                    var client = await server.Accept();
                    Console.WriteLine("Client connected " + client);
                    StartClient(client);
                }
            }
        }

        private static async Task StartClient(NetSocket client)
        {
            Console.WriteLine("Client Task started");
            var buf = new byte[2550];
            try
            {
                while (true)
                {
                    var count = await client.Read(buf);
                    Console.WriteLine(count + " bytes received.");
                    /*if (count > 200)
                    {
                        Console.WriteLine("Disconnecting client " + client);
                        await client.Disconnect();
                        Console.WriteLine("Client disconnected");
                        break;
                    }*/
                }
            }
            catch (Exception ex)
            {
                var se = ex as SocketException;
                if (se != null && se.SocketErrorCode == SocketError.Shutdown)
                {
                    Console.WriteLine("Client Task Stopped");
                    return;
                }
                Console.WriteLine("Exception in client thread " + ex);
                throw;
            }
            finally
            {
                client.Dispose();
            }
        }

        private static void Main(string[] args)
        {
            //Work();
            EventLoop.Run(Work);
            Console.ReadLine();
        }
    }
}