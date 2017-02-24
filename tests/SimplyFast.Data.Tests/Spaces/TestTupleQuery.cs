using SimplyFast.Data.Spaces.Interface;

namespace SimplyFast.Data.Tests.Spaces
{
    public class TestTupleQuery : IQuery<TestTuple>
    {
        public TestTupleQuery(int? x, int? y)
        {
            X = x;
            Y = y;
        }

        public int? X { get; }
        public int? Y { get; }

        public TupleType Type => new TupleType(0);

        public bool Match(TestTuple tuple)
        {
            return (!X.HasValue || X.Value == tuple.X)
                   && (!Y.HasValue || Y.Value == tuple.Y);
        }
    }
}