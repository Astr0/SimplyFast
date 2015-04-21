using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Reflection
{
    internal class FieldInfoCache
    {
        private static readonly ConcurrentDictionary<Type, FieldInfoCache> _fieldsCache = new ConcurrentDictionary<Type, FieldInfoCache>();

        // ReSharper disable MemberHidesStaticFromOuterClass
        public readonly FieldInfo[] Fields;
        // ReSharper restore MemberHidesStaticFromOuterClass
        private readonly Dictionary<string, FieldInfo> _fields;

        private FieldInfoCache(IReflect type)
        {
            Fields = type.GetFields(MemberInfoEx.BindingFlags);
            _fields = Fields.ToDictionary(x => x.Name, StringComparer.Ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfoCache ForType(Type type)
        {
            return _fieldsCache.GetOrAdd(type, t => new FieldInfoCache(t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldInfo Get(string name)
        {
            FieldInfo field;
            _fields.TryGetValue(name, out field);
            return field;
        }
    }
}