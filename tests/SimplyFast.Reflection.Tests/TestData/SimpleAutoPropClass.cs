using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SimplyFast.Reflection.Tests.TestData
{
    public class SimpleAutoPropClass
    {
        [CompilerGenerated]
        public string SomeField;

        [SuppressMessage("ReSharper", "UnusedMember.Global")] 
        public string SomeProp { get; set; }
    }
}