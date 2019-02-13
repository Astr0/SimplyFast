using System.Collections.Concurrent;

namespace SimplyFast.Log.Internal
{
    internal class LoggerFactory : ILoggerFactory
    {
        private readonly ConcurrentDictionary<object, ILogger> _loggers = new ConcurrentDictionary<object, ILogger>();

        public LoggerFactory(ILogger root)
        {
            Root = root;
        }

        public ILogger Root { get; }

        public ILogger Get(object key)
        {
            return key != null
                ? _loggers.GetOrAdd(key, x => new ChildLogger(x.ToString(), Root))
                : Root;
        }
    }
}