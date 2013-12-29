using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Reflection
{
    internal static class MethodDelegateCache
    {
        private static readonly ConcurrentDictionary<Tuple<MethodInfo, Type>, object> _delegateCache = new ConcurrentDictionary<Tuple<MethodInfo, Type>, object>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object InvokerAs(MethodInfo methodInfo, Type delegateType)
        {
            return _delegateCache.GetOrAdd(Tuple.Create(methodInfo, delegateType),
                                           t => new DelegateBuilders.MethodDelegateBuilder(t.Item1, t.Item2).CreateDelegate());
        }
    }
}