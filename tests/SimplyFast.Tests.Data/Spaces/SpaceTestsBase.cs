using NUnit.Framework;
using SF.Data.Spaces;

namespace SF.Tests.Data.Spaces
{
    public abstract class SpaceTestsBase
    {
        public abstract ISyncSpaceTable<TestTuple> SyncSpaceTable { get; }

        [Test]
        public void CanWriteObjectToSpace()
        {
            var tuple = new TestTuple(1, 0);
            SyncSpaceTable.Add(tuple);
            var result = SyncSpaceTable.TryRead(new TestTupleQuery(1, null));
            Assert.AreEqual(tuple, result);
        }

        [Test]
        public void CanReadObjectFromSpace()
        {
            var tuple = new TestTuple(1, 0);
            var tuple2 = new TestTuple(2, 0);
            SyncSpaceTable.Add(tuple);
            SyncSpaceTable.Add(tuple2);
            var result = SyncSpaceTable.TryRead(new TestTupleQuery(1, null));
            Assert.AreEqual(tuple, result);
            result = SyncSpaceTable.TryRead(new TestTupleQuery(1, 0));
            Assert.AreEqual(tuple, result);
            result = SyncSpaceTable.TryRead(new TestTupleQuery(2, null));
            Assert.AreEqual(tuple2, result);
            result = SyncSpaceTable.TryRead(new TestTupleQuery(3, null));
            Assert.IsNull(result);
        }

    }
}