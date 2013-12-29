using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Reflection
{
    public static class MethodInfoEx
    {
        /// <summary>
        ///     Checks if method is operator
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOperator(this MethodInfo methodInfo)
        {
            return methodInfo.IsSpecialName && methodInfo.Name.StartsWith("op_");
        }

        /// <summary>
        ///     Returns Property who's getter or setter is passed method
        /// </summary>
        public static PropertyInfo FindParentProperty(MethodInfo method)
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
        ///     MakeGenericMethod with cache
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo MakeGeneric(this MethodInfo method, params Type[] arguments)
        {
            return GenericMethodInfoCache.MakeGeneric(method, arguments);
        }

        /// <summary>
        ///     Returns all methods of type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo[] Methods(this Type type)
        {
            return MethodInfoCache.ForType(type).Methods;
        }

        /// <summary>
        ///     Returns all method of type with passed name
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo[] Methods(this Type type, string name)
        {
            return MethodInfoCache.ForType(type).Get(name);
        }

        /// <summary>
        ///     Returns random method of type with passed name
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo Method(this Type type, string name)
        {
            return MethodInfoCache.ForType(type).First(name);
        }

        /// <summary>
        ///     Finds method of type with passed name and passed arguments
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo Method(this Type type, string name, params Type[] arguments)
        {
            return MethodInfoCache.ForType(type).Get(name, arguments);
        }

        /// <summary>
        ///     Finds generic method with matched name, generic args and arguments, Use Tx for substituting generic arguments
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo Method(this Type type, string name, int genericArgCount, params Type[] arguments)
        {
            return MethodInfoCache.ForType(type).GetSubstituted(name, genericArgCount, arguments);
        }

        /// <summary>
        ///     Finds generic method that will have passed arguments if used with passed genericArguments
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo Method(this Type type, string name, Type[] genericArguments, Type[] arguments)
        {
            return MethodInfoCache.ForType(type).GetGeneric(name, genericArguments, arguments);
        }

        /// <summary>
        /// Returns delegate's invoke method
        /// </summary>
        public static MethodInfo GetInvokeMethod(Type delegateType)
        {
            if (delegateType.BaseType != typeof(MulticastDelegate))
                throw new ArgumentException("Not a delegate", "delegateType");
            var invokeMethod = delegateType.Method("Invoke");
            if (invokeMethod == null)
                throw new ArgumentException("Not a delegate", "delegateType");
            return invokeMethod;
        }

        /// <summary>
        ///     Returns method invoker as delegateType
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object InvokerAs(this MethodInfo methodInfo, Type delegateType)
        {
            return MethodDelegateCache.InvokerAs(methodInfo, delegateType);
        }

        /// <summary>
        ///     Returns method invoker as TDelegate
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TDelegate InvokerAs<TDelegate>(this MethodInfo methodInfo)
            where TDelegate : class
        {
            return (TDelegate) MethodDelegateCache.InvokerAs(methodInfo, typeof (TDelegate));
        }
    }
}