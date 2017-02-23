using System;

namespace SF
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