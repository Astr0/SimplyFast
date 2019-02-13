namespace SimplyFast.Log
{
    public static class SeverityEx
    {
        public static bool ShouldLog(this Severity logSeverity, IMessage message)
        {
            return logSeverity.ShouldLog(message.Severity);
        }

        public static bool ShouldLog(this Severity logSeverity, Severity messageSeverity)
        {
            return logSeverity <= messageSeverity && messageSeverity < Severity.Off;
        }
    }
}