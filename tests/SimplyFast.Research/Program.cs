using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SF.IO;
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
            var pipe = new ProducerConsumerPipe<string>();

            var tasks = new List<Task>
            {
                Producer(pipe), 
                Consumer(pipe),
                Consumer(pipe),
                Producer(pipe), 
                Consumer(pipe),
                Consumer(pipe),
                Producer(pipe), 
                Consumer(pipe),
            };

            await Task.WhenAll(tasks);
        }

        private static async Task Producer(IProducer producer)
        {
            while (true)
            {
                await producer.Add("test");
                await Task.Delay(500);
            }
        }

        private static async Task Consumer(IConsumer consumer)
        {
            while (true)
            {
                var str = await consumer.Take<string>();
                Console.WriteLine("Consumed " + str);
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