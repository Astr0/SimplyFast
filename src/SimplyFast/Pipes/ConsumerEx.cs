//using System;

//namespace SF.Pipes
//{
//    public static class ConsumerEx
//    {
//        /// <summary>
//        /// Creates consumer from event pattern. Action will block if nothing taken
//        /// </summary>
//        public static IConsumer<T> FromEvent<T>(Func<Action<T>, IDisposable> subscribe)
//        {
//            return new EventConsumer<T>(subscribe);
//        }
//    }
//}