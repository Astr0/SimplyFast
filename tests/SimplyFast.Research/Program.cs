using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SimplyFast.Research
{
    internal class Program
    {
        public static void DebugWrite(string caption)
        {
            Console.WriteLine(caption + " " + Thread.CurrentThread.ManagedThreadId + " " + TaskScheduler.Current.Id);
        }

        public static async Task<int> GetA()
        {
            await Task.Delay(1000);
            DebugWrite("GetA");
            return 0;
        }

        public static async Task<int> GetSum()
        {
            DebugWrite("GetSum - before GetA");
            var res = await Task.WhenAll(GetA(), GetA());
            var a = res[0];
            var b = res[1];
            DebugWrite("GetSum - after GetA");
            return a + b;
        }

        private static async void Work()
        {
            DebugWrite("Main - before GetSum");
            var res = await GetSum();
            Console.WriteLine(res);
        }

        public class TestSyncContext : SynchronizationContext
        {
            private struct QueueItem
            {
                public QueueItem(SendOrPostCallback callback, object state)
                {
                    _callback = callback;
                    _state = state;
                }

                public void Execute()
                {
                    _callback(_state);
                }

                private readonly SendOrPostCallback _callback;
                private readonly object _state;
            }

            private readonly ConcurrentQueue<QueueItem> _queue = new ConcurrentQueue<QueueItem>();
            private volatile int _ops;


            public override void OperationCompleted()
            {
                Console.WriteLine("OperationCompleted");
                Interlocked.Decrement(ref _ops);
            }

            public override void OperationStarted()
            {
                Console.WriteLine("OperationStarted");
                Interlocked.Increment(ref _ops);
            }

            public override int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
            {
                Console.WriteLine("Wait");
                return base.Wait(waitHandles, waitAll, millisecondsTimeout);
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                Console.WriteLine("Send");
                d(state);
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                DebugWrite("Post " + d.Method);
                _queue.Enqueue(new QueueItem(d, state));
            }

            public void Run()
            {
                while (!_queue.IsEmpty || _ops > 0)
                {
                    QueueItem item;
                    if (!_queue.TryDequeue(out item))
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    try
                    {
                        item.Execute();
                    }
                    catch
                    {
                        
                    }
                }
            }
        }


        private static void Main(string[] args)
        {
            var context = new TestSyncContext();
            SynchronizationContext.SetSynchronizationContext(context);
            context.Post(x => Work(), null);
            context.Run();
            Console.ReadLine();
        }
    }
}