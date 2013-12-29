using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Reflection
{
    public static class ContructorInfoEx
    {
        /// <summary>
        ///     Creates instance of type using parametless constructor and casts it to T
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object CreateInstance(this Type type)
        {
            return type.Constructor().InvokerAs<Func<object>>()();
        }

        /// <summary>
        ///     Creates instance of type using parametless constructor and casts it to T
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateInstance<T>(this Type type)
        {
            return type.Constructor().InvokerAs<Func<T>>()();
        }

        /// <summary>
        ///     Creates instance of T using parametless constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateInstance<T>()
        {
            return typeof (T).CreateInstance<T>();
        }

        /// <summary>
        ///     Returns all constructors for passed type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInfo[] Constructors(this Type type)
        {
            return ConstructorInfoCache.ForType(type).Constructors;
        }

        /// <summary>
        ///     Returns constructors with matched arguments or null
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInfo Constructor(this Type type, params Type[] arguments)
        {
            return ConstructorInfoCache.ForType(type).Get(arguments);
        }

        /// <summary>
        ///     Builds delegate of delegateType for invoking constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object InvokerAs(this ConstructorInfo constructorInfo, Type delegateType)
        {
            return ConstructorDelegateCache.InvokerAs(constructorInfo, delegateType);
        }

        /// <summary>
        ///     Builds delegate of TDelegate for invoking constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TDelegate InvokerAs<TDelegate>(this ConstructorInfo constructorInfo)
            where TDelegate : class
        {
            return (TDelegate)ConstructorDelegateCache.InvokerAs(constructorInfo, typeof(TDelegate));
        }
    }
}