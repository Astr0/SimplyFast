using System;
using SimplyFast.Cache;
using SimplyFast.Log.Internal.Loggers;

namespace SimplyFast.Log
{
    public class LoggerFactory
    {
        private readonly ICache<object, ILogger> _childLoggers = CacheEx.ThreadSafe<object, ILogger>();
        private readonly Func<Severity, IMessage> _messageFactory;

        public LoggerFactory(Func<Severity, IMessage> messageFactory = null)
        {
            _messageFactory = messageFactory ?? MessageEx.Default;
            Root = new Logger("Root", _messageFactory);
        }

        public ILogger Root { get; }

        public ILogger Get(object key)
        {
            return key != null
                ? _childLoggers.GetOrAdd(key, k => new ChildLogger(Root, k.ToString(), _messageFactory))
                : Root;
        }
    }
}