using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using SimplyFast.Cache;
using SimplyFast.Reflection.Internal.DelegateBuilders;

namespace SimplyFast.Reflection.Internal
{
    internal static class FieldDelegateCache
    {
        private static readonly ICache<Tuple<FieldInfo, Type>, Delegate> _getCache =
            CacheEx.ThreadSafe<Tuple<FieldInfo, Type>, Delegate>();

        private static readonly ICache<Tuple<FieldInfo, Type>, Delegate> _setCache =
            CacheEx.ThreadSafe<Tuple<FieldInfo, Type>, Delegate>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Delegate GetterAs(FieldInfo fieldInfo, Type delegateType)
        {
            return _getCache.GetOrAdd(Tuple.Create(fieldInfo, delegateType), t => new FieldGetDelegateBuilder(fieldInfo, delegateType).CreateDelegate());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Delegate SetterAs(FieldInfo fieldInfo, Type delegateType)
        {
            return fieldInfo.CanWrite()
                ? _setCache.GetOrAdd(Tuple.Create(fieldInfo, delegateType), t => new FieldSetDelegateBuilder(fieldInfo, delegateType).CreateDelegate())
                : null;
        }
    }
}