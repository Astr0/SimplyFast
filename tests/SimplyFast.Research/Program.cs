using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SF.Threading;

namespace SimplyFast.Research
{
    internal class Program
    {
        public static void DebugWrite(string caption)
        {
            Console.WriteLine(caption + " " + Thread.CurrentThread.ManagedThreadId + " " + TaskScheduler.Current.Id);
        }

        public static async Task<int> GetA()
        {
            //await Task.Delay(1000);
            //DebugWrite("GetA");
            return 0;
        }

        public static async Task<int> GetSum()
        {
            //DebugWrite("GetSum - before GetA");
            var res = await Task.WhenAll(GetA(), GetA());
            var a = res[0];
            var b = res[1];
            //DebugWrite("GetSum - after GetA");
            return a + b;
        }

        private static async void Work()
        {
            await TestCatch();
            //DebugWrite("Main - before GetSum");
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000000; i++)
            {
                var res = await GetSum();
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            
            //Console.WriteLine(res);
        }

        private static async Task TestCatch()
        {
            try
            {
                await SomethingThrows();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Catched " + ex.Message);
            }
        }

        private static Task SomethingThrows()
        {
            throw new Exception("My Ex");
        }


        private static void Main(string[] args)
        {
            //Work();
            EventLoop.Run(Work);
            Console.ReadLine();
        }
    }
}