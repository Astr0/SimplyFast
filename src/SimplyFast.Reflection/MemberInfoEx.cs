using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SimplyFast.Reflection
{
    public static class MemberInfoEx
    {
        #region Private Access

#if REFLECTIONEX
        private static bool _privateAccess;
        private static BindingFlags _bindingFlags;

        static MemberInfoEx()
        {
            PrivateAccess = true;
        }

        public static bool PrivateAccess
        {
            get { return _privateAccess; }
            set
            {
                _privateAccess = value;

                _bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
                if (_privateAccess)
                    _bindingFlags |= BindingFlags.NonPublic;
            }
        }

        internal static BindingFlags BindingFlags
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _bindingFlags; }
        }
#else
        public static bool PrivateAccess => false;
#endif

#endregion

        /// <summary>
        ///     Checks if member can be written to
        /// </summary>
        public static bool CanWrite(this MemberInfo memberInfo)
        {
#if NET
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) memberInfo).CanWrite();
                case MemberTypes.Property:
                    return ((PropertyInfo) memberInfo).CanWrite;
            }
            return false;
#else
            var field = memberInfo as FieldInfo;
            if (field != null)
                return field.CanWrite();
            var property = memberInfo as PropertyInfo;
            return property != null && property.CanWrite;
#endif
        }

        /// <summary>
        ///     Checks if member can be read
        /// </summary>
        public static bool CanRead(this MemberInfo memberInfo)
        {
#if NET
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return true;
                case MemberTypes.Property:
                    return ((PropertyInfo) memberInfo).CanRead;
            }
            return false;
#else
            if (memberInfo is FieldInfo)
                return true;
            var propertyInfo = memberInfo as PropertyInfo;
            return propertyInfo != null && propertyInfo.CanRead;
#endif
        }

        /// <summary>
        ///     Finds field or property wiht passed name
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MemberInfo FieldOrProperty(this Type type, string name)
        {
            return (MemberInfo)type.Field(name) ?? type.Property(name);
        }

        /// <summary>
        ///     Returns ValueType for field or property
        /// </summary>
        public static Type ValueType(this MemberInfo member)
        {
#if NET
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo) member).PropertyType;
            }
#else
            var field = member as FieldInfo;
            if (field != null)
                return field.FieldType;
            var property = member as PropertyInfo;
            if (property != null)
                return property.PropertyType;
#endif
            throw new ArgumentException("Not a field or property.", nameof(member));
        }

        /// <summary>
        ///     Returns Method, generic method, delegate field or property with matched name and parameters
        /// </summary>
        public static MemberInfo FindInvokableMember(this Type type, string member, params Type[] parameters)
        {
            var method = type.FindMethod(member, parameters);
            if (method != null)
                return method;
            var fieldOrProperty = type.FieldOrProperty(member);
            if (fieldOrProperty == null || !fieldOrProperty.CanRead())
                return null;
            var valueType = fieldOrProperty.ValueType();
            try
            {
                var invokeParameters = MethodInfoEx.GetInvokeMethod(valueType).GetParameters();
                // check parameters
                return !invokeParameters.Select(x => x.ParameterType).SequenceEqual(parameters) ? null : fieldOrProperty;
            }
            catch
            {
                return null;
            }
        }
    }
}