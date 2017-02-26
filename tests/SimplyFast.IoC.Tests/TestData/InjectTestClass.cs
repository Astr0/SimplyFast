using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC.Tests.TestData
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class InjectTestClass
    {
        public long Long;
        public string String;
        public List<long> Longs;

        [Inject]
        public void Init(long value)
        {
            Long = value;
        }

        [Inject]
        public void Init(List<long> longs)
        {
            Longs = longs;
        }

        [Inject]
        public void Init(long value, List<long> longs)
        {
            Long = value;
            Longs = longs;
        }

        [Inject]
        public void Init(long value, List<long> longs, string str)
        {
            Init(value, longs);
            String = str;
        }
    }
}