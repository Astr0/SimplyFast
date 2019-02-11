using System.Reflection;
using System.Runtime.CompilerServices;
using SimplyFast.Cache;

namespace SimplyFast.Reflection.Internal
{
    internal static class ConstructorInvokerCache
    {
        private static readonly ICache<ConstructorInfo, ConstructorInvoker> _delegateCache =
            CacheEx.ThreadSafe<ConstructorInfo, ConstructorInvoker>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInvoker Get(ConstructorInfo constructorInfo)
        {
            return _delegateCache.GetOrAdd(constructorInfo, InvokerDelegateBuilder.Current.BuildConstructorInvoker);
        }
    }
}