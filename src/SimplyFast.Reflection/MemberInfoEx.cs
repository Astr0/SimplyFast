using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SimplyFast.Reflection
{
    public static class MemberInfoEx
    {
        static MemberInfoEx()
        {
            PrivateAccess = true;
        }

        // TODO: Clear all caches on change?
        public static bool PrivateAccess { get; set; }
        
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
                default:
                    return false;
            }
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
                default:
                    return false;
            }
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
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo) member).PropertyType;
            }
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

        public static bool HasAttribute<T>(this MemberInfo member) where T: Attribute
        {
            return Attribute.IsDefined(member, typeof(T));
        }

        public static bool CompilerGenerated(this MemberInfo member)
        {
            return member.HasAttribute<CompilerGeneratedAttribute>();
        }
    }
}