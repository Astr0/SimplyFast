using System.Threading;

namespace SF.Threading
{
    internal sealed class EventLoopSynchronizationContext : SynchronizationContext
    {
        public readonly EventLoopImplementation EventLoopImplementation;

        public EventLoopSynchronizationContext(EventLoopImplementation eventLoopImplementation)
        {
            EventLoopImplementation = eventLoopImplementation;
        }

        public override SynchronizationContext CreateCopy()
        {
            return new EventLoopSynchronizationContext(EventLoopImplementation);
        }

        public override void OperationStarted()
        {
            EventLoopImplementation.OperationStarted();
        }

        public override void OperationCompleted()
        {
            EventLoopImplementation.OperationCompleted();
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            EventLoopImplementation.Send(d, state);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            EventLoopImplementation.Post(d, state);
        }
    }
}