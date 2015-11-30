namespace SF.Data.Spaces
{
    public static class SpaceFactory
    {
        /// <summary>
        /// Creates local space that doesn't clone stuff, so changing what was received from space suxx
        /// It's also single-threaded which doesn't make much sense for practical usage
        /// </summary>
        public static ILocalSpace<T> UnsafeLocal<T>() where T : class
        {
            return new LocalSpace<T>();
        }
    }
}