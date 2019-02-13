//using System;
//using SimplyFast.Log.Messages;
//using SimplyFast.Log.Messages.Internal;

//namespace SimplyFast.Log
//{
//    public static class Logger
//    {
//        private static volatile ILoggerFactory _factory = LoggerEx.CreateDefaultFactory();
//        private static volatile IMessageFactory _messageFactory = new DefaultMessageFactory();

//        public static ILoggerFactory Factory
//        {
//            get => _factory;
//            set => _factory = value ?? LoggerEx.CreateDefaultFactory();
//        }

//        public static IMessageFactory MessageFactory
//        {
//            get => _messageFactory;
//            set => _messageFactory = value ?? new DefaultMessageFactory();
//        }

//        public static ILogger Get(string name)
//        {
//            return _factory.Get(name);
//        }

//        public static ILogger Get(Type type)
//        {
//            return _factory.Get(type);
//        }
//    }
//}