using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SimplyFast.Reflection.Internal
{
    internal static class ContructorInvokerCache
    {
        private static readonly ConcurrentDictionary<ConstructorInfo, ConstructorInvoker> _delegateCache =
            new ConcurrentDictionary<ConstructorInfo, ConstructorInvoker>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInvoker Get(ConstructorInfo constructorInfo)
        {
            return _delegateCache.GetOrAdd(constructorInfo, InvokerDelegateBuilder.BuildConstructorInvoker);
        }
    }
}