using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Reflection
{
    internal static class GenericMethodInfoCache
    {
        private static readonly ConcurrentDictionary<GenericMethodKey, MethodInfo> _genericCache =
            new ConcurrentDictionary<GenericMethodKey, MethodInfo>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo MakeGeneric(MethodInfo method, params Type[] arguments)
        {
            return _genericCache.GetOrAdd(new GenericMethodKey(method, arguments), k => k.Method.MakeGenericMethod(k.Arguments));
        }

        #region Nested type: GenericMethodKey

        private struct GenericMethodKey : IEquatable<GenericMethodKey>
        {
            private static readonly EqualityComparer<Type[]> _comparer = SimpleEqualityComparer.Array<Type>();

            public readonly Type[] Arguments;
            // ReSharper disable MemberHidesStaticFromOuterClass
            public readonly MethodInfo Method;
            // ReSharper restore MemberHidesStaticFromOuterClass

            public GenericMethodKey(MethodInfo method, Type[] arguments)
            {
                Method = method;
                Arguments = arguments;
            }

            #region IEquatable<GenericMethodKey> Members

            public bool Equals(GenericMethodKey other)
            {
                return Method == other.Method && _comparer.Equals(Arguments, other.Arguments);
            }

            #endregion

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != GetType()) return false;
                return Equals((GenericMethodKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Method.GetHashCode()*397) ^ _comparer.GetHashCode(Arguments);
                }
            }
        }

        #endregion
    }
}