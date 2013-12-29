﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace SF.Reflection
{
    public static class MemberInfoEx
    {
        /// <summary>
        /// Checks if member can be written to
        /// </summary>
        public static bool CanWrite(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).CanWrite();
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).CanWrite;
            }
            return false;
        }

        /// <summary>
        /// Checks if member can be read
        /// </summary>
        public static bool CanRead(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return true;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).CanRead;
            }
            return false;
        }

        /// <summary>
        /// Finds field or property wiht passed name
        /// </summary>
        public static MemberInfo FieldOrProperty(this Type type, string name)
        {
            return (MemberInfo)type.Field(name) ?? type.Property(name);
        }

        /// <summary>
        /// Returns ValueType for field or property
        /// </summary>
        public static Type ValueType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException("Not a field or property.", "member");
            }
        }

        /// <summary>
        /// Returns members that have custom attribute
        /// </summary>
        public static IEnumerable<KeyValuePair<T, Attribute>> WithAttribute<T>(this IEnumerable<T> members, Type attributeType, bool inherit = true)
            where T : MemberInfo
        {
            foreach (var member in members)
            {
                var attribute = member.GetCustomAttribute(attributeType, inherit);
                if (attribute != null)
                    yield return new KeyValuePair<T, Attribute>(member, attribute);
            }
        }
    }
}