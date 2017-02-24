using SimplyFast.Data.Spaces.Impl.Local;
using SimplyFast.Data.Spaces.Interface;

namespace SimplyFast.Data.Spaces
{
    public static class SpaceFactory
    {
        /// <summary>
        ///     Creates local space that doesn't clone stuff, so changing what was received from space suxx
        ///     It's also single-threaded which doesn't make much sense for practical usage
        /// </summary>
        public static ISpace UnsafeLocal()
        {
            return new LocalSpace();
        }
    }
}