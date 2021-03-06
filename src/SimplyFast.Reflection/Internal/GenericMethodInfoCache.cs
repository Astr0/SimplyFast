﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using SimplyFast.Cache;
using SimplyFast.Comparers;

namespace SimplyFast.Reflection.Internal
{
    internal static class GenericMethodInfoCache
    {
        private static readonly ICache<GenericMethodKey, MethodInfo> _genericCache =
            CacheEx.ThreadSafe<GenericMethodKey, MethodInfo>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo MakeGeneric(MethodInfo method, params Type[] arguments)
        {
            return _genericCache.GetOrAdd(new GenericMethodKey(method, arguments), k => k.Method.MakeGenericMethod(k.Arguments));
        }

        #region Nested type: GenericMethodKey

        private struct GenericMethodKey : IEquatable<GenericMethodKey>
        {
            private static readonly EqualityComparer<Type[]> _comparer = EqualityComparerEx.Array<Type>();

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
                return ReferenceEquals(Method, other.Method) && _comparer.Equals(Arguments, other.Arguments);
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