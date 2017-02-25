using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SimplyFast.Reflection.Internal
{
    internal class PropertyInfoCache
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfoCache> _propertyCache = new ConcurrentDictionary<Type, PropertyInfoCache>();

        public readonly string IndexerName;
        // ReSharper disable MemberHidesStaticFromOuterClass
        public readonly PropertyInfo[] Properties;
        // ReSharper restore MemberHidesStaticFromOuterClass
        private readonly Dictionary<string, PropertyInfo[]> _properties;

        private PropertyInfoCache(Type type)
        {
            Properties = type.AllProperties();
            _properties = Properties.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToArray(), StringComparer.Ordinal);
            var defaultMember = type.TypeInfo().GetCustomAttribute<DefaultMemberAttribute>();
            if (defaultMember != null)
                IndexerName = defaultMember.MemberName;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfoCache ForType(Type type)
        {
            return _propertyCache.GetOrAdd(type, t => new PropertyInfoCache(t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PropertyInfo[] Get(string name)
        {
            PropertyInfo[] properties;
            _properties.TryGetValue(name, out properties);
            return properties;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PropertyInfo First(string name)
        {
            PropertyInfo[] properties;
            return _properties.TryGetValue(name, out properties) ? properties[0] : null;
        }

        public PropertyInfo GetIndexed(string name, Type[] parameters)
        {
            PropertyInfo[] properties;
            if (!_properties.TryGetValue(name, out properties))
                return null;
            foreach (var property in properties)
            {
                var args = property.GetIndexParameters();
                if (args.Length != parameters.Length)
                    continue;
                var found = true;
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i] == args[i].ParameterType) continue;
                    found = false;
                    break;
                }
                if (found)
                    return property;
            }
            return null;
        }
    }
}