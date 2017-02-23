using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Reflection.Internal
{
    internal static class MethodInvokerCache
    {
        private static readonly ConcurrentDictionary<MethodInfo, MethodInvoker> _delegateCache =
            new ConcurrentDictionary<MethodInfo, MethodInvoker>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInvoker Get(MethodInfo methodInfo)
        {
            return _delegateCache.GetOrAdd(methodInfo, InvokerDelegateBuilder.BuildMethodInvoker);
        }
    }
}