using System;
using SF.Reflection;

namespace SimplyFast.Research
{
    public class WeakDelegateTest:ResearchBase
    {
        public class WeakDelegateString : WeakDelegate<Action<string>>
        {
            private void Invoke(string x)
            {
                foreach (var del in this)
                {
                    del(x);
                }
            }

            protected override Action<string> BuildInvoker()
            {
                return Invoke;
            }
        }

        private string Test(string x, string y)
        {
            return x + y;
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