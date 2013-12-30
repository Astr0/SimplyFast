using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SF.Reflection
{
    /// <summary>
    ///     Facade to Type related stuff
    /// </summary>
    public static class TypeEx
    {
        /// <summary>
        /// Returns programmer-friendly name for type
        /// </summary>
        public static string FriendlyName(this Type type)
        {
            if (!type.IsGenericType)
                return type.Name;
            var sb = new StringBuilder();
            var arguments = type.GetGenericArguments();
            var genericName = type.Name;
            var index = genericName.IndexOf('`');
            if (index >= 0)
                genericName = genericName.Substring(0, index);
            sb.Append(genericName).Append('<').Append(string.Join(", ", arguments.Select(x => x.FriendlyName()))).Append('>');
            return sb.ToString();
        }

        /// <summary>
        ///     Replaces generic type arguments in passed type using substitution function
        /// </summary>
        /// <param name="type">Type to replace</param>
        /// <param name="substitution">Function that returns new type for passed type</param>
        /// <returns>Type with all generic arguments substituted</returns>
        public static Type Substitute(Type type, Func<Type, Type> substitution)
        {
            var sub = substitution(type);
            if (sub != null && sub != type)
                return sub;

            if (!type.IsGenericType)
                return type;

            var args = type.GetGenericArguments();
            var changed = false;
            for (var i = 0; i < args.Length; i++)
            {
                var newType = Substitute(args[i], substitution);
                if (newType == args[i])
                    continue;
                args[i] = newType;
                changed = true;
            }
            if (changed)
                type = type.GetGenericTypeDefinition().MakeGeneric(args);
            return type;
        }

        /// <summary>
        ///     Returns underlying type for ByRef type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type RemoveByRef(this Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }

        /// <summary>
        ///     Returns declared type of an object. Usefull for reflection with anonymous.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type TypeOf<T>(T obj)
        {
            return typeof (T);
        }
        
        /// <summary>
        /// Returns all underlying types for IEnumerable implementations found in type
        /// </summary>
        public static IEnumerable<Type> FindIEnumerable(Type type)
        {
            var interfaces = type.GetInterfaces().Concat(new []{ type });
            var normalFound = false;
            foreach (var inter in interfaces)
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    yield return inter.GetGenericArguments()[0];
                }
                else if (inter == typeof(IEnumerable))
                    normalFound = true;
            }
            if (normalFound)
                yield return typeof(object);
        }

        /// <summary>
        ///     Returns generic type implementation in passed type
        /// </summary>
        /// <param name="typeDefinition">Type definition to search</param>
        /// <param name="type">Type to search for implementation</param>
        /// <returns>First implementation found or null if not found</returns>
        public static Type FindGenericType(Type typeDefinition, Type type)
        {
            while (type != null && type != typeof (object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeDefinition)
                    return type;
                if (typeDefinition.IsInterface)
                {
                    foreach (var interfaceType in type.GetInterfaces())
                    {
                        var foundType = FindGenericType(typeDefinition, interfaceType);
                        if (foundType != null)
                            return foundType;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        ///     MakeGenericType with cache lookup
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type MakeGeneric(this Type type, params Type[] arguments)
        {
            return GenericTypeCache.MakeGeneric(type, arguments);
        }

        /// <summary>
        ///     Cached Type.GetType or search in all assemblies in current AppDomain
        /// </summary>
        /// <param name="name">Assembly qualified type name or full type name</param>
        /// <returns>Type if found or null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ResolveType(string name)
        {
            return TypeResolveCache.Resolve(name);
        }
    }
}