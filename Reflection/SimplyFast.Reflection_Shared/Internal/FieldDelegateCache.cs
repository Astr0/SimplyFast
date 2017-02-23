using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using SF.Reflection.DelegateBuilders;

namespace SF.Reflection
{
    internal static class FieldDelegateCache
    {
        private static readonly ConcurrentDictionary<Tuple<FieldInfo, Type>, object> _getCache =
            new ConcurrentDictionary<Tuple<FieldInfo, Type>, object>();

        private static readonly ConcurrentDictionary<Tuple<FieldInfo, Type>, object> _setCache =
            new ConcurrentDictionary<Tuple<FieldInfo, Type>, object>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetterAs(FieldInfo fieldInfo, Type delegateType)
        {
            return _getCache.GetOrAdd(Tuple.Create(fieldInfo, delegateType), t => new FieldGetDelegateBuilder(fieldInfo, delegateType).CreateDelegate());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object SetterAs(FieldInfo fieldInfo, Type delegateType)
        {
            return fieldInfo.CanWrite()
                ? _setCache.GetOrAdd(Tuple.Create(fieldInfo, delegateType), t => new FieldSetDelegateBuilder(fieldInfo, delegateType).CreateDelegate())
                : null;
        }
    }
}