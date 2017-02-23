using NUnit.Framework;
using SF.Data.Spaces;

namespace SF.Tests.Data.Spaces
{
    [TestFixture]
    public class LocalSpaceBasicTests: SpaceTestsBase
    {
        private ISpace _space;
        private ISpaceProxy _spaceProxy;
        [SetUp]
        public void Setup()
        {
            _space = SpaceFactory.UnsafeLocal();
            _spaceProxy = _space.CreateProxy();
        }

        public override ISpace Space => _space;
        public override ISpaceProxy SpaceProxy => _spaceProxy;
    }
}