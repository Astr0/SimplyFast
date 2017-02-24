using System;

namespace SimplyFast
{
    public static class GCEx
    {
        public static void CollectAndWait()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}