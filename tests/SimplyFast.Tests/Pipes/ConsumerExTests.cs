using System;
using NUnit.Framework;

namespace SF.Tests.Pipes
{
    [TestFixture]
    public class ConsumerExTests
    {
        public class EventSource
        {
            public event Action<int> Signal;

            public virtual void OnSignal(int obj)
            {
                Action<int> handler = Signal;
                if (handler != null) handler(obj);
            }

            public bool Empty { get { return Signal == null; } }
        }

        //[Test]
        //public void FromEventWorks()
        //{
        //    var es = new EventSource();

        //    var sum = 0;
        //    var i = 0;
        //    var can = 0;
        //    var iOk = false;
        //    var task = Task.Run(async () =>
        //    {
        //        using (var signal = ConsumerEx.FromEvent<int>(x =>
        //        {
        //            es.Signal += x;
        //            return DisposableEx.Action(() => es.Signal -= x);
        //        }))
        //        {
        //            var t = Task.Run(() =>
        //            {
        //                for (i = 0; i < 10; ++i)
        //                    es.OnSignal(i);
        //            });

        //            for (var j = 0; j < 10; ++j)
        //            {
        //                sum += await signal.Take();
        //            }
        //            iOk = (i == 10);
        //            using (var cts = new CancellationTokenSource(100))
        //            {
        //                try
        //                {
        //                    sum += await signal.Take(cts.Token);
        //                }
        //                catch (OperationCanceledException)
        //                {
        //                    can++;
        //                }
        //            }
        //            es.OnSignal(11);
        //            try
        //            {
        //                sum += await signal.Take(new CancellationToken(true));
        //            }
        //            catch (OperationCanceledException)
        //            {
        //                can++;
        //            }
        //            sum += await signal.Take();
        //            await t;
        //        }
        //    });
        //    task.Wait();
        //    Assert.IsTrue(iOk);
        //    Assert.AreEqual(2, can);
        //    Assert.AreEqual(Enumerable.Range(0, 11).Sum(), sum);
        //    Assert.IsTrue(es.Empty);
        //}
    }
}