using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using SimplyFast.Cache;
using SimplyFast.Reflection.Internal.DelegateBuilders;

namespace SimplyFast.Reflection.Internal
{
    internal static class MethodDelegateCache
    {
        private static readonly ICache<Tuple<MethodInfo, Type>, Delegate> _delegateCache =
            CacheEx.ThreadSafe<Tuple<MethodInfo, Type>, Delegate>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Delegate InvokerAs(MethodInfo methodInfo, Type delegateType)
        {
            return _delegateCache.GetOrAdd(Tuple.Create(methodInfo, delegateType),
                t => DelegateBuilder.Current.Method(t.Item1, t.Item2));
        }
    }
}