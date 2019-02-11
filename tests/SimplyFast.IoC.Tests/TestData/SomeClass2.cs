using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC.Tests.TestData
{

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    internal class SomeClass2
    {
        public List<int> Ints { get; }
        public SomeClass Test { get; }

        public SomeClass2(List<int> ints, SomeClass test)
        {
            Ints = ints;
            Test = test;
        }
    }
}