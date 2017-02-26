using Xunit;
using SimplyFast.Data.Spaces;
using SimplyFast.Data.Spaces.Interface;

namespace SimplyFast.Data.Tests.Spaces
{
    
    public class LocalSpaceBasicTests: SpaceTestsBase
    {
        private readonly ISpace _space;

        public LocalSpaceBasicTests()
        {
            _space = SpaceFactory.UnsafeLocal();
            SpaceProxy = _space.CreateProxy();
        }

        public override ISpace Space => _space;
        public override ISpaceProxy SpaceProxy { get; }
    }
}