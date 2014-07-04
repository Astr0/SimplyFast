namespace SF.Logging
{
    public static class LoggerEx
    {
        public static ILogger Null()
        {
            return NullLoger.Instance;
        }
    }
}