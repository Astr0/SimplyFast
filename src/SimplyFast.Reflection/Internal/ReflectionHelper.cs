using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimplyFast.Comparers;

namespace SimplyFast.Reflection.Internal
{
    internal static class ReflectionHelper
    {
        public static ConstructorInfo[] AllConstructors(this Type type)
        {
            var pa = MemberInfoEx.PrivateAccess;
            return type.TypeInfo().DeclaredConstructors.Where(x => !x.IsStatic && (pa || x.IsPublic)).ToArray();
        }

        public static FieldInfo[] AllFields(this Type type)
        {
            var result = new List<FieldInfo>();
            var pa = MemberInfoEx.PrivateAccess;
            var ti = type.TypeInfo();
            var set = new HashSet<string>();

            foreach (var field in ti.DeclaredFields)
                if ((pa || field.IsPublic) && set.Add(field.Name))
                    result.Add(field);

            while (true)
            {
                var baseType = ti.BaseType;
                if (baseType == null)
                    break;
                ti = baseType.TypeInfo();

                foreach (var field in ti.DeclaredFields)
                {
                    if (field.IsStatic)
                        continue;
                    if (field.IsPrivate)
                        continue;
                    if ((pa || field.IsPublic) && set.Add(field.Name))
                        result.Add(field);
                }
            }
            return result.ToArray();
        }

        public static MethodInfo[] AllMethods(this Type type)
        {
            var result = new List<MethodInfo>();
            var pa = MemberInfoEx.PrivateAccess;
            var ti = type.TypeInfo();
            var set = new HashSet<MethodKey>();

            foreach (var method in ti.DeclaredMethods)
                if ((pa || method.IsPublic) && set.Add(new MethodKey(method)))
                    result.Add(method);

            while (true)
            {
                var baseType = ti.BaseType;
                if (baseType == null)
                    break;
                ti = baseType.TypeInfo();

                foreach (var method in ti.DeclaredMethods)
                {
                    if (method.IsStatic)
                        continue;
                    if (method.IsPrivate)
                        continue;
                    if ((pa || method.IsPublic) && set.Add(new MethodKey(method)))
                        result.Add(method);
                }
            }
            return result.ToArray();
        }

        public static PropertyInfo[] AllProperties(this Type type)
        {
            var result = new List<PropertyInfo>();
            var pa = MemberInfoEx.PrivateAccess;
            var ti = type.TypeInfo();
            var set = new HashSet<MethodKey>();

            foreach (var property in ti.DeclaredProperties)
                if ((pa || property.IsPublic()) && set.Add(new MethodKey(property)))
                    result.Add(property);

            while (true)
            {
                var baseType = ti.BaseType;
                if (baseType == null)
                    break;
                ti = baseType.TypeInfo();

                foreach (var property in ti.DeclaredProperties)
                {
                    if (property.IsStatic())
                        continue;
                    if (property.IsPrivate())
                        continue;
                    if ((pa || property.IsPublic()) && set.Add(new MethodKey(property)))
                        result.Add(property);
                }
            }
            return result.ToArray();
        }
        
        private struct MethodKey : IEquatable<MethodKey>
        {
            private static readonly EqualityComparer<ParameterInfo[]> ParametersComparer =
                EqualityComparerEx.Array<ParameterInfo>();

            public bool Equals(MethodKey other)
            {
                return string.Equals(_name, other._name) && ParametersComparer.Equals(_parameters, other._parameters);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is MethodKey && Equals((MethodKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_name.GetHashCode() * 397) ^ ParametersComparer.GetHashCode(_parameters);
                }
            }

            private readonly string _name;
            private readonly ParameterInfo[] _parameters;

            public MethodKey(MethodInfo method) : this()
            {
                _name = method.Name;
                _parameters = method.GetParameters();
            }

            public MethodKey(PropertyInfo property) : this()
            {
                _name = property.Name;
                _parameters = property.GetIndexParameters();
            }
        }
    }
}