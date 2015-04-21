using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Reflection
{
    public static class MemberInfoEx
    {
        #region Private Access

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

        public static BindingFlags BindingFlags
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _bindingFlags; }
        }

        #endregion

        /// <summary>
        ///     Checks if member can be written to
        /// </summary>
        public static bool CanWrite(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) memberInfo).CanWrite();
                case MemberTypes.Property:
                    return ((PropertyInfo) memberInfo).CanWrite;
            }
            return false;
        }

        /// <summary>
        ///     Checks if member can be read
        /// </summary>
        public static bool CanRead(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return true;
                case MemberTypes.Property:
                    return ((PropertyInfo) memberInfo).CanRead;
            }
            return false;
        }

        /// <summary>
        ///     Finds field or property wiht passed name
        /// </summary>
        public static MemberInfo FieldOrProperty(this Type type, string name)
        {
            return (MemberInfo) type.Field(name) ?? type.Property(name);
        }

        /// <summary>
        ///     Returns ValueType for field or property
        /// </summary>
        public static Type ValueType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo) member).PropertyType;
                default:
                    throw new ArgumentException("Not a field or property.", "member");
            }
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