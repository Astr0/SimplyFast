using System;
using System.Diagnostics;
using System.Threading;

namespace SimplyFast.Research
{
    public abstract class PerfTestBase
    {
        protected readonly int Iterations;

        protected PerfTestBase()
            : this(10000000)
        {
        }

        protected PerfTestBase(int iterations)
        {
            Iterations = iterations;
        }

        protected abstract void DoRun();

        protected static void TestPerformance(Action action, int iterations, bool jitPrepare)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            TestPerformance(action, iterations, action.Method.Name, jitPrepare);
        }

        protected static void TestPerformance(Action action, int iterations, string caption, bool jitPrepare)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (jitPrepare)
            {
                // JIT prepare
                if (iterations > 0)
                {
                    var jitPrepareCount = Math.Max(1, iterations/100);
                    for (var i = 0; i < jitPrepareCount; i++)
                    {
                        action();
                    }
                }
            }

            var sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            for (var i = 0; i < iterations; i++)
            {
                action();
            }
            sw.Stop();
            if (iterations != 1)
                Console.WriteLine("{0}({1} iterations) - {2} ms.", caption, iterations, sw.ElapsedMilliseconds);
            else
                Console.WriteLine("{0} - {1} ms.", caption, sw.ElapsedMilliseconds);
        }

        protected void TestPerformance(Action action, string caption)
        {
            TestPerformance(action, 1, caption + " (" + Iterations + " iterations)", false);
        }

        protected void TestSinglePerformance(Action action, string caption)
        {
            TestPerformance(action, 1, caption, false);
        }

        public void Run()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            var typeName = GetType().Name;
            var starsLeft = (Console.WindowWidth - typeName.Length)/2;
            Console.Write(new string('*', starsLeft));
            Console.Write(typeName);
            Console.WriteLine(new string('*', Console.WindowWidth - starsLeft));
            DoRun();
            Console.WriteLine();
            Console.WriteLine(new string('-', Console.WindowWidth));
        } 
    }
}