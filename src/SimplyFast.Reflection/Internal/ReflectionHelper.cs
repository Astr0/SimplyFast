using System;
using System.Reflection;
using SimplyFast.Comparers;
#if !REFLECTIONEX
using System.Linq;
using System.Collections.Generic;
#endif

namespace SimplyFast.Reflection.Internal
{
    internal static class ReflectionHelper
    {
        public static ConstructorInfo[] AllConstructors(this Type type)
        {
#if REFLECTIONEX
            return type.TypeInfo().GetConstructors(MemberInfoEx.BindingFlags & ~BindingFlags.Static);
#else
            var pa = MemberInfoEx.PrivateAccess;
            return GetAllConstructors(type).Where(x => pa || x.IsPublic).ToArray();
#endif
        }

        public static FieldInfo[] AllFields(this Type type)
        {
#if REFLECTIONEX
            return type.TypeInfo().GetFields(MemberInfoEx.BindingFlags);
#else
            var pa = MemberInfoEx.PrivateAccess;
            return GetAllFields(type).Where(x => pa || x.IsPublic).ToArray();
#endif
        }

        public static MethodInfo[] AllMethods(this Type type)
        {
#if REFLECTIONEX
            return type.TypeInfo().GetMethods(MemberInfoEx.BindingFlags);
#else
            var pa = MemberInfoEx.PrivateAccess;
            return GetAllMethods(type).Where(x => pa || x.IsPublic).ToArray();
#endif
        }

        public static PropertyInfo[] AllProperties(this Type type)
        {
#if REFLECTIONEX
            return type.TypeInfo().GetProperties(MemberInfoEx.BindingFlags);
#else
            var pa = MemberInfoEx.PrivateAccess;
            return GetAllProperties(type).Where(x => pa || x.IsPublic()).ToArray();
#endif
        }

#if !REFLECTIONEX

        private static IEnumerable<ConstructorInfo> GetAllConstructors(Type type)
        {
            return type.TypeInfo().DeclaredConstructors.Where(x => !x.IsStatic);
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            var ti = type.TypeInfo();
            var set = new HashSet<string>();

            foreach (var field in ti.DeclaredFields)
            {
                if (set.Add(field.Name))
                    yield return field;
            }

            while (true)
            {
                var baseType = ti.BaseType;
                if (baseType == null)
                    yield break;
                ti = baseType.TypeInfo();

                foreach (var field in ti.DeclaredFields)
                {
                    if (field.IsStatic)
                        continue;
                    if (field.IsPrivate)
                        continue;
                    if (set.Add(field.Name))
                        yield return field;
                }
            }
        }

        private static IEnumerable<PropertyInfo> GetAllProperties(Type type)
        {
            var ti = type.TypeInfo();
            var set = new HashSet<MethodKey>();

            foreach (var property in ti.DeclaredProperties)
            {
                if (set.Add(new MethodKey(property)))
                    yield return property;
            }

            while (true)
            {
                var baseType = ti.BaseType;
                if (baseType == null)
                    yield break;
                ti = baseType.TypeInfo();

                foreach (var property in ti.DeclaredProperties)
                {
                    if (property.IsStatic())
                        continue;
                    if (property.IsPrivate())
                        continue;
                    if (set.Add(new MethodKey(property)))
                        yield return property;
                }
            }
        }

        private static IEnumerable<MethodInfo> GetAllMethods(Type type)
        {
            var ti = type.TypeInfo();
            var set = new HashSet<MethodKey>();

            foreach (var method in ti.DeclaredMethods)
            {
                if (set.Add(new MethodKey(method)))
                    yield return method;
            }

            while (true)
            {
                var baseType = ti.BaseType;
                if (baseType == null)
                    yield break;
                ti = baseType.TypeInfo();

                foreach (var method in ti.DeclaredMethods)
                {
                    if (method.IsStatic)
                        continue;
                    if (method.IsPrivate)
                        continue;
                    if (set.Add(new MethodKey(method)))
                        yield return method;
                }
            }
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
                return obj is MethodKey && Equals((MethodKey)obj);
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
#endif
    }
}