using SimplyFast.Cache;

namespace SimplyFast.Log.Internal.Loggers
{
    public class LoggerFactory
    {
        public LoggerFactory(ILogger root)
        {
            Root = root;
        }

        public LoggerFactory(): this(new Logger("Root"))
        {
        }

        public ILogger Root { get; }

        private readonly ICache<object, ILogger> _childLoggers = CacheEx.ThreadSafe<object, ILogger>();

        public ILogger Get(object key)
        {
            return key != null ? _childLoggers.GetOrAdd(key, k => new ChildLogger(Root, k.ToString())) : Root;
        }
    }
}