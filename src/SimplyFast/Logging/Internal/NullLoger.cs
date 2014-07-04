using System;

namespace SF.Logging
{
    internal sealed class NullLoger: ILogger
    {
        public static readonly ILogger Instance = new NullLoger();

        private NullLoger()
        {
            
        }

        public void Info(string message)
        {
        }

        public void Debug(string message)
        {
        }

        public void Warning(string message)
        {
        }

        public void Error(string message, Exception ex = null)
        {
        }

        public void Fatal(string message, Exception ex = null)
        {
        }
    }
}