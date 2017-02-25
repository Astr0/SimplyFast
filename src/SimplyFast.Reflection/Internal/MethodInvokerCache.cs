using System.Reflection;
using System.Runtime.CompilerServices;
using SimplyFast.Cache;

namespace SimplyFast.Reflection.Internal
{
    internal static class MethodInvokerCache
    {
        private static readonly ICache<MethodInfo, MethodInvoker> _delegateCache =
            CacheEx.ThreadSafe<MethodInfo, MethodInvoker>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInvoker Get(MethodInfo methodInfo)
        {
            return _delegateCache.GetOrAdd(methodInfo, InvokerDelegateBuilder.BuildMethodInvoker);
        }
    }
}