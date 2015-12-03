using System;
using System.Diagnostics;
using System.Threading;
using SimplyFast.Research.Spaces;

namespace SimplyFast.Research
{
    internal class Program
    {
        private static void Main()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            //ZmqTest.Work();
            //EventLoop.Run(CastleZmqTest.Work);
            //new WeakStuffTest().Run();
            //new EmitTest().Run();
            new LegacyLocalSpaceWriteTests().Run();
            new LocalSpaceWriteTests().Run();
            Console.ReadLine();
        }
    }
}