using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimplyFast.Research
{
    public class TestClass
    {
        [IndexerName("Tst")]
        public int this[int index]
        {
            get
            {
                return index;
            }
        }

        [IndexerName("Tst")]
        public int this[string index]
        {
            get
            {
                return 0;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
