using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SF.Threading
{
    public static class EventLoop
    {
        private static EventLoopImplementation CurrentLoop
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var elContext = SynchronizationContext.Current as EventLoopSynchronizationContext;
                if (elContext == null)
                    throw new InvalidOperationException("Event Loop is not running on current thread");
                return elContext.EventLoopImplementation;
            }
        }

        /// <summary>
        ///     Runs event loop on current thread
        /// </summary>
        /// <param name="init">First event to be invoked</param>
        /// <param name="state">Event parameter</param>
        public static void Run(SendOrPostCallback init, object state)
        {
            using (var loop = new EventLoopImplementation())
            {
                var oldContext = SynchronizationContext.Current;
                var loopContext = new EventLoopSynchronizationContext(loop);
                try
                {
                    // Install loop context
                    SynchronizationContext.SetSynchronizationContext(loopContext);

                    // Run loop
                    loop.Run(init, state);
                }
                finally
                {
                    // Restore sync context
                    SynchronizationContext.SetSynchronizationContext(oldContext);
                }
            }
        }

        /// <summary>
        ///     Runs event loop on current thread
        /// </summary>
        /// <param name="init">First event to be invoked</param>
        public static void Run(Action init)
        {
            Run(x => init(), null);
        }

        public static void DoEvents()
        {
            CurrentLoop.DoEvents();
        }

        public static event Action<Exception> UnhandledException
        {
            add { CurrentLoop.UnhandledException += value; }
            remove { CurrentLoop.UnhandledException -= value; }
        }
    }
}