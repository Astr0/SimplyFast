using System;
using SF.Threading;

namespace SimplyFast.Research
{
    internal class Program
    {
        private static void Main()
        {
            //ZmqTest.Work();
            EventLoop.Run(ZmqTest.Work);
            Console.ReadLine();
        }
    }
}