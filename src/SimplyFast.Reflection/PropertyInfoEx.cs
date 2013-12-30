using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Reflection
{
    public static class PropertyInfoEx
    {
        /// <summary>
        ///     Checks if property is static
        /// </summary>
        public static bool IsStatic(this PropertyInfo propertyInfo)
        {
            var method = propertyInfo.GetGetMethod(SimpleReflection.PrivateAccess);
            if (method != null)
                return method.IsStatic;
            method = propertyInfo.GetSetMethod(SimpleReflection.PrivateAccess);
            return method.IsStatic;
        }

        /// <summary>
        ///     Checks if property getter or setter is public
        /// </summary>
        public static bool IsPublic(this PropertyInfo propertyInfo)
        {
            var get = propertyInfo.GetGetMethod();
            if (get != null && get.IsPublic)
                return true;
            var set = propertyInfo.GetSetMethod();
            if (set != null && set.IsPublic)
                return true;
            return false;
        }

        /// <summary>
        ///     Returns all properties of type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo[] Properties(this Type type)
        {
            return PropertyInfoCache.ForType(type).Properties;
        }

        /// <summary>
        ///     Returns all properties of type with passed name
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo[] Properties(this Type type, string name)
        {
            return PropertyInfoCache.ForType(type).Get(name);
        }

        /// <summary>
        ///     Returns random property with passed name
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo Property(this Type type, string name)
        {
            return PropertyInfoCache.ForType(type).First(name);
        }

        /// <summary>
        ///     Returns indexed property with passed name and arguments
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo Property(this Type type, string name, params Type[] parameters)
        {
            return PropertyInfoCache.ForType(type).GetIndexed(name, parameters);
        }

        /// <summary>
        ///     Returns Property who's getter or setter is passed method
        /// </summary>
        public static PropertyInfo Property(MethodInfo method)
        {
            // Easy and fast way out. 
            if (!method.IsSpecialName)
                return null;

            // Try euristics
            if (method.Name.Length > 4)
            {
                // Property getter/setter name is get_x or set_x
                var prefix = method.Name.Substring(0, 4);
                var getSet = (prefix == "get_" ? 1 : (prefix == "set_" ? 2 : 0));
                if (getSet != 0)
                {
                    var propertyName = method.Name.Substring(4);
                    var properties = method.DeclaringType.Properties(propertyName);
                    return
                        properties.FirstOrDefault(
                            x => (getSet == 1 ? x.GetGetMethod(SimpleReflection.PrivateAccess) : x.GetSetMethod(SimpleReflection.PrivateAccess)) == method);
                }
            }
            // ReSharper disable once PossibleNullReferenceException
            var allProperties = method.DeclaringType.Properties();

            // Euristics failed... try the hard way
            return
                allProperties.FirstOrDefault(
                    p => p.GetGetMethod(SimpleReflection.PrivateAccess) == method || p.GetSetMethod(SimpleReflection.PrivateAccess) == method);
        }

        /// <summary>
        ///     Returns default index property with parameters
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo Indexer(this Type type, params Type[] parameters)
        {
            var cache = PropertyInfoCache.ForType(type);
            return cache.IndexerName == null ? null : cache.GetIndexed(cache.IndexerName, parameters);
        }

        /// <summary>
        ///     Returns property getter delegate as delegateType
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetterAs(PropertyInfo propertyInfo, Type delegateType)
        {
            return propertyInfo.CanRead ? propertyInfo.GetGetMethod(SimpleReflection.PrivateAccess).InvokerAs(delegateType) : null;
        }

        /// <summary>
        ///     Returns property setter delegate as delegateType
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object SetterAs(PropertyInfo propertyInfo, Type delegateType)
        {
            return propertyInfo.CanWrite ? propertyInfo.GetSetMethod(SimpleReflection.PrivateAccess).InvokerAs(delegateType) : null;
        }

        /// <summary>
        ///     Returns property getter delegate as TDelegate
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TDelegate GetterAs<TDelegate>(this PropertyInfo propertyInfo)
            where TDelegate : class
        {
            return (TDelegate) GetterAs(propertyInfo, typeof (TDelegate));
        }

        /// <summary>
        ///     Returns property setter delegate as TDelegate
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TDelegate SetterAs<TDelegate>(this PropertyInfo propertyInfo)
            where TDelegate : class
        {
            return (TDelegate) SetterAs(propertyInfo, typeof (TDelegate));
        }
    }
}