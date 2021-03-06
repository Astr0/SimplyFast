﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SimplyFast.Cache;

namespace SimplyFast.Reflection.Internal
{
    internal class FieldInfoCache
    {
        private static readonly ICache<Type, FieldInfoCache> _fieldsCache = CacheEx.ThreadSafe<Type, FieldInfoCache>();

        // ReSharper disable MemberHidesStaticFromOuterClass
        public readonly FieldInfo[] Fields;
        // ReSharper restore MemberHidesStaticFromOuterClass
        private readonly Dictionary<string, FieldInfo> _fields;

        private FieldInfoCache(Type type)
        {
            Fields = type.AllFields();
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