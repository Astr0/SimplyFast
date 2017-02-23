using System;

namespace SimplyFast.Research
{
    public class EmitTest:ResearchBase
    {
        public int Test(IDisposable x)
        {
            using (x)
            {
                return 42;
            }
        }

        public void Run()
        {
            /*var wd = WeakDelegate<Func<string, string, string>>.Create();
            Console.WriteLine(wd.Invoker("test", "1"));
            wd.Add(Test);
            wd.Add(Test);
            Console.WriteLine(wd.Invoker("test", "2"));*/
            //Console.WriteLine(wd.Invoker);
        }
    }
}