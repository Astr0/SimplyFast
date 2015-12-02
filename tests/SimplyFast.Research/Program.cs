using System;

namespace SimplyFast.Research
{
    internal class Program
    {
        private static void Main()
        {
            //ZmqTest.Work();
            //EventLoop.Run(CastleZmqTest.Work);
            //new WeakStuffTest().Run();
            //new EmitTest().Run();
            new LocalSpaceWriteTests().Run();
            Console.ReadLine();
        }
    }
}