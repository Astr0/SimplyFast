using NUnit.Framework;
using SF.Data.Spaces;

namespace SF.Tests.Data.Spaces
{
    [TestFixture]
    public class LocalSpaceBasicTests: SpaceTestsBase
    {
        private ISyncSpace _space;
        private ISyncSpaceTable<TestTuple> _spaceTable;
        [SetUp]
        public void Setup()
        {
            _space = SpaceFactory.UnsafeLocal();
            _spaceTable = _space.GetTable<TestTuple>(0);
        }

        public override ISyncSpace SyncSpace => _space;
        public override ISyncSpaceTable<TestTuple> SyncSpaceTable => _spaceTable;
    }
}