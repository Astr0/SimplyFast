using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SimplyFast.Reflection.Internal;

namespace SimplyFast.Reflection
{
    public static class FieldInfoEx
    {
        /// <summary>
        ///     Checks if field can be written to
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
        public static Delegate GetterAs(this FieldInfo fieldInfo, Type delegateType)
        {
            return FieldDelegateCache.GetterAs(fieldInfo, delegateType);
        }

        /// <summary>
        ///     Returns field setter delegate as delegateType
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Delegate SetterAs(this FieldInfo fieldInfo, Type delegateType)
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
            return (TDelegate) (object) FieldDelegateCache.GetterAs(fieldInfo, typeof(TDelegate));
        }

        /// <summary>
        ///     Returns field setter delegate as TDelegate
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TDelegate SetterAs<TDelegate>(this FieldInfo fieldInfo)
            where TDelegate : class
        {
            return (TDelegate) (object) FieldDelegateCache.SetterAs(fieldInfo, typeof(TDelegate));
        }

        /// <summary>
        /// Returns Declaring property for backing fields
        /// </summary>
        public static PropertyInfo DeclaringProperty(this FieldInfo fieldInfo)
        {
            if (!fieldInfo.CompilerGenerated())
                return null;
            const string end = ">k__BackingField";
            var name = fieldInfo.Name;
            if (string.IsNullOrEmpty(name) || name[0] != '<' || !name.EndsWith(end))
                return null;
            var propertyName = name.Substring(1, name.Length - end.Length - 1);
            return fieldInfo.DeclaringType.Property(propertyName);
        }
    }
}