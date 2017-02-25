using System;
using System.Reflection;
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
            return type.TypeInfo().DeclaredConstructors.Where(x => !x.IsStatic).ToArray();
#endif
        }

#if !REFLECTIONEX
        private static IEnumerable<T> FromBase<T>(this TypeInfo typeInfo, Func<TypeInfo, IEnumerable<T>> get)
            where T: MemberInfo
        {
            var ti = typeInfo;
            while (true)
            {
                foreach (var t in get(ti))
                {
                    yield return t;
                }
                var baseType = ti.BaseType;
                if (baseType == null)
                    yield break;
                ti = baseType.TypeInfo();
            }
        }
#endif

        public static FieldInfo[] AllFields(this Type type)
        {
#if REFLECTIONEX
            return type.TypeInfo().GetFields(MemberInfoEx.BindingFlags);
#else
            return type.TypeInfo().FromBase(t => t.DeclaredFields).ToArray();
#endif
        }

        public static MethodInfo[] AllMethods(this Type type)
        {
#if REFLECTIONEX
            return type.TypeInfo().GetMethods(MemberInfoEx.BindingFlags);
#else
            MethodInfo m;
            return type.TypeInfo().FromBase(t => t.DeclaredMethods).ToArray();
#endif
        }

        public static PropertyInfo[] AllProperties(this Type type)
        {
#if REFLECTIONEX
            return type.TypeInfo().GetProperties(MemberInfoEx.BindingFlags);
#else
            return type.TypeInfo().FromBase(t => t.DeclaredProperties).ToArray();
#endif
        }
    }
}