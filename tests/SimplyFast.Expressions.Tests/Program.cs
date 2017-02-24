using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NUnit.Common;
using NUnitLite;

namespace SF.Tests
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Program
    {
        public static int Main(string[] args)
        {
            var result = new AutoRun(typeof(Program).GetTypeInfo().Assembly)
                .Execute(args, new ExtendedTextWrapper(Console.Out), Console.In);
            if (result != 0)
                Console.ReadKey();
            return result;
        }
    }
}