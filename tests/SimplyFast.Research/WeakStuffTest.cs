using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace SimplyFast.Research
{
    public class WeakStuffTest: PerfTestBase
    {
        private class IntContainer
        {
            public readonly int Value;

            public IntContainer(int value)
            {
                Value = value;
            }
        }

        public WeakStuffTest() : base(100000)
        {
        }

        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        protected override void DoRun()
        {
            var live = Enumerable.Range(0, 1000).Select(x => new IntContainer(x)).ToArray();
            List<WeakReference> weak;
            List<WeakReference<IntContainer>> weakT;
            List<GCHandle> gcHandles;
            {
                var all = live.Concat(Enumerable.Range(0, 1000).Select(x => new IntContainer(x))).ToArray();
                weak = new List<WeakReference>(all.Select(x => new WeakReference(x)));
                weakT = new List<WeakReference<IntContainer>>(all.Select(x => new WeakReference<IntContainer>(x)));
                gcHandles = new List<GCHandle>(all.Select(x => GCHandle.Alloc(x, GCHandleType.Weak)));
            }
            GC.Collect();
            GC.WaitForFullGCComplete();

            TestPerformance(() =>
            {
                weak.Sum(x =>
                {
                    var t = x.Target;
                    if (t == null)
                        return 0;
                    return ((IntContainer) t).Value;
                });
            }, Iterations, "Weak", true);

            TestPerformance(() =>
            {
                weakT.Sum(x =>
                {
                    IntContainer t;
                    return x.TryGetTarget(out t) ? t.Value : 0;
                });
            }, Iterations, "WeakT", true);

            TestPerformance(() =>
            {
                gcHandles.Sum(x =>
                {
                    var t = x.Target;
                    if (t == null)
                        return 0;
                    return ((IntContainer)t).Value;
                });
            }, Iterations, "GCHandle", true);
        }
    }
}