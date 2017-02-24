using NUnit.Framework;
using SimplyFast.Data.Spaces;
using SimplyFast.Data.Spaces.Interface;

namespace SimplyFast.Data.Tests.Spaces
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