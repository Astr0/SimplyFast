using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SimplyFast.Cache;
using SimplyFast.Comparers;

namespace SimplyFast.Reflection.Internal
{
    internal static class GenericTypeCache
    {
        private static readonly ICache<GenericTypeKey, Type> _genericCache = CacheEx.ThreadSafe<GenericTypeKey, Type>();

        /// <summary>
        ///     MakeGenericType with cache lookup
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type MakeGeneric(Type type, params Type[] arguments)
        {
            return _genericCache.GetOrAdd(new GenericTypeKey(type, arguments), k => k.Type.MakeGenericType(k.Arguments));
        }

        #region Nested type: GenericTypeKey

        private struct GenericTypeKey : IEquatable<GenericTypeKey>
        {
            private static readonly EqualityComparer<Type[]> _comparer = EqualityComparerEx.Array<Type>();
            public readonly Type[] Arguments;
            public readonly Type Type;

            public GenericTypeKey(Type type, Type[] arguments)
            {
                Type = type;
                Arguments = arguments;
            }

            #region IEquatable<GenericTypeKey> Members

            public bool Equals(GenericTypeKey other)
            {
                return Type == other.Type && _comparer.Equals(Arguments, other.Arguments);
            }

            #endregion

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != GetType()) return false;
                return Equals((GenericTypeKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Type.GetHashCode() * 397) ^ _comparer.GetHashCode(Arguments);
                }
            }
        }

        #endregion
    }
}