using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Reflection
{
    public static class FieldInfoEx
    {
        /// <summary>
        ///     Checks if field can be writter to
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanWrite(this FieldInfo fieldInfo)
        {
            return !(fieldInfo.IsInitOnly || fieldInfo.IsLiteral);
        }

        /// <summary>
        ///     Returns all fields of passed type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo[] Fields(this Type type)
        {
            return FieldInfoCache.ForType(type).Fields;
        }

        /// <summary>
        ///     Returns field with passed name or null
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo Field(this Type type, string name)
        {
            return FieldInfoCache.ForType(type).Get(name);
        }

        /// <summary>
        ///     Returns field getter delegate as delegateType
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetterAs(this FieldInfo fieldInfo, Type delegateType)
        {
            return FieldDelegateCache.GetterAs(fieldInfo, delegateType);
        }

        /// <summary>
        ///     Returns field setter delegate as delegateType
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object SetterAs(this FieldInfo fieldInfo, Type delegateType)
        {
            return FieldDelegateCache.SetterAs(fieldInfo, delegateType);
        }

        /// <summary>
        ///     Returns field getter delegate as TDelegate
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TDelegate GetterAs<TDelegate>(this FieldInfo fieldInfo)
            where TDelegate : class
        {
            return (TDelegate) FieldDelegateCache.GetterAs(fieldInfo, typeof (TDelegate));
        }

        /// <summary>
        ///     Returns field setter delegate as TDelegate
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TDelegate SetterAs<TDelegate>(this FieldInfo fieldInfo)
            where TDelegate : class
        {
            return (TDelegate) FieldDelegateCache.SetterAs(fieldInfo, typeof (TDelegate));
        }
    }
}