using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SimplyFast.Comparers
{
    internal class ReferenceEqualityComparer: EqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance
            = new ReferenceEqualityComparer(); 

        private ReferenceEqualityComparer() { }

        public override bool Equals(object x, object y)
        {
            // object == is called in ReferenceEquals
            return x == y; 
        }

        public override int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}