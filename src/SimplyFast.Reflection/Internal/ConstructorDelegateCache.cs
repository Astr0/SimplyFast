using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using SimplyFast.Reflection.Internal.DelegateBuilders;

namespace SimplyFast.Reflection.Internal
{
    internal static class ConstructorDelegateCache
    {
        private static readonly ConcurrentDictionary<Tuple<ConstructorInfo, Type>, object> _delegateCache =
            new ConcurrentDictionary<Tuple<ConstructorInfo, Type>, object>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object InvokerAs(ConstructorInfo constructorInfo, Type delegateType)
        {
            return _delegateCache.GetOrAdd(Tuple.Create(constructorInfo, delegateType),
                t => new ConstructorDelegateBuilder(t.Item1, t.Item2).CreateDelegate());
        }
    }
}