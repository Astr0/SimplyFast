using System;
using System.Threading;

namespace SF.Data.Legacy.Spaces
{
    public static class SpaceFactory
    {
        /// <summary>
        ///     Creates local space that doesn't clone stuff, so changing what was received from space suxx
        ///     It's also single-threaded which doesn't make much sense for practical usage
        /// </summary>
        public static ISyncSpace UnsafeLocal()
        {
            return new LocalSpace();
        }

        /// <summary>
        ///     Creates local space that completely safe and implements both space interfaces
        /// </summary>
        public static ISpace SafeLocal<T>(Converter<object, object> clone, SynchronizationContext context = null)
            where T : class
        {
            return new SafeLocalSpace(UnsafeLocal(), clone, context);
        }
    }
}