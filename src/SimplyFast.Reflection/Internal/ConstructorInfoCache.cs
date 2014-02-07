using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Reflection
{
    internal class ConstructorInfoCache
    {
        private static readonly ConcurrentDictionary<Type, ConstructorInfoCache> _constructorCache = new ConcurrentDictionary<Type, ConstructorInfoCache>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInfoCache ForType(Type type)
        {
            return _constructorCache.GetOrAdd(type, t => new ConstructorInfoCache(t));
        }

        // ReSharper disable MemberHidesStaticFromOuterClass
        public readonly ConstructorInfo[] Constructors;
        // ReSharper restore MemberHidesStaticFromOuterClass
        private readonly Dictionary<Type[], ConstructorInfo> _constructors;

        private ConstructorInfoCache(Type type)
        {
            Constructors = type.GetConstructors(SimpleReflection.BindingFlags & ~BindingFlags.Static);
            _constructors = new Dictionary<Type[], ConstructorInfo>(EqualityComparerEx.Array<Type>());
            foreach (var constructorInfo in Constructors)
            {
                _constructors[constructorInfo.GetParameters().Select(x => x.ParameterType).ToArray()] = constructorInfo;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ConstructorInfo Get(Type[] types)
        {
            ConstructorInfo result;
            _constructors.TryGetValue(types, out result);
            return result;
        }
    }
}