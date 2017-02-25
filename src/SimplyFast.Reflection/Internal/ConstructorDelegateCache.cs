using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using SimplyFast.Cache;
using SimplyFast.Reflection.Internal.DelegateBuilders;

namespace SimplyFast.Reflection.Internal
{
    internal static class ConstructorDelegateCache
    {
        private static readonly ICache<Tuple<ConstructorInfo, Type>, Delegate> _delegateCache = CacheEx.ThreadSafe<Tuple<ConstructorInfo, Type>, Delegate>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Delegate InvokerAs(ConstructorInfo constructorInfo, Type delegateType)
        {
            return _delegateCache.GetOrAdd(Tuple.Create(constructorInfo, delegateType),
                t => new ConstructorDelegateBuilder(t.Item1, t.Item2).CreateDelegate());
        }
    }
}