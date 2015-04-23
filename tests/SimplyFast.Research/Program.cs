using System;
using SF.Threading;

namespace SimplyFast.Research
{
    internal class Program
    {
        private static void Main()
        {
            //ZmqTest.Work();
            //EventLoop.Run(CastleZmqTest.Work);
            //new WeakStuffTest().Run();
            new EmitTest().Run();
            Console.ReadLine();
        }
    }
}