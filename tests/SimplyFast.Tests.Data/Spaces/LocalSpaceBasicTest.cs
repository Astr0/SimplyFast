using NUnit.Framework;
using SF.Data.Spaces;

namespace SF.Tests.Data.Spaces
{
    [TestFixture]
    public class LocalSpaceBasicTests: SpaceTestsBase
    {
        private ISyncSpaceTable<TestTuple> _spaceTable;
        [SetUp]
        public void Setup()
        {
            _spaceTable = SpaceFactory.UnsafeLocal().GetTable<TestTuple>(0);
        }

        public override ISyncSpaceTable<TestTuple> SyncSpaceTable => _spaceTable;
    }
}