using System;
using System.Runtime.Serialization;

namespace SF.Pipes
{
    [Serializable]
    public class EndOfPipeException: Exception
    {
        public EndOfPipeException()
        {
        }

        public EndOfPipeException(string message) : base(message)
        {
        }

        public EndOfPipeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EndOfPipeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}