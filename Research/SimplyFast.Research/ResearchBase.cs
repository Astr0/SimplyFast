using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimplyFast.Research
{
    public class ResearchBase
    {
        public static void DebugWrite(string caption)
        {
            Console.WriteLine(caption + " " + Thread.CurrentThread.ManagedThreadId + " " + TaskScheduler.Current.Id);
        }
    }
}