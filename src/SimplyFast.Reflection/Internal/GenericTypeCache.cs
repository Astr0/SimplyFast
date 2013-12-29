using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SF.Reflection
{
    internal static class GenericTypeCache
    {
        private static readonly ConcurrentDictionary<GenericTypeKey, Type> _genericCache = new ConcurrentDictionary<GenericTypeKey, Type>();

        private struct GenericTypeKey : IEquatable<GenericTypeKey>
        {
            private static readonly EqualityComparer<Type[]> _comparer = SimpleEqualityComparer.Array<Type>();
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
                return (Type == other.Type && _comparer.Equals(Arguments, other.Arguments));
            }

            #endregion

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != GetType()) return false;
                return Equals((GenericTypeKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Type.GetHashCode() * 397) ^ _comparer.GetHashCode(Arguments);
                }
            }
        }

        /// <summary>
        ///     MakeGenericType with cache lookup
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type MakeGeneric(Type type, params Type[] arguments)
        {
            return _genericCache.GetOrAdd(new GenericTypeKey(type, arguments), k => k.Type.MakeGenericType(k.Arguments));
        }
    }
}