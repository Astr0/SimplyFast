using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC.Tests.TestData
{

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    internal class TestClass2
    {
        public List<int> Ints { get; }
        public TestClass Test { get; }

        public TestClass2(List<int> ints, TestClass test)
        {
            Ints = ints;
            Test = test;
        }
    }
}