using System;

namespace SF
{
    public interface ILogger
    {
        void Info(string message);
        void Debug(string message);
        void Warning(string message);
        void Error(string message, Exception ex = null);
        void Fatal(string message, Exception ex = null);
    }
}