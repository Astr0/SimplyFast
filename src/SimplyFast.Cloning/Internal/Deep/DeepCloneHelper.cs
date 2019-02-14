using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimplyFast.Reflection;

namespace SimplyFast.Cloning.Internal.Deep
{
    internal static class DeepCloneHelper
    {
        private static IEnumerable<CloneFieldInfo> GetTypeFields(Type type)
        {
            return type
                .Fields()
                .Where(x => !x.IsStatic && x.DeclaringType == type && x.CanWrite())
                .Select(x =>
                {
                    var attrMember = (MemberInfo)x.DeclaringProperty() ?? x;

                    var clone = CloneObjectEx.GetCloneTypeFromAttribute(attrMember) ??
                                CloneObjectEx.GetCloneType(x.FieldType);

                    return new CloneFieldInfo(x, clone);
                });
        }

        public static IEnumerable<CloneFieldInfo> GetDeepCloneFields(Type type)
        {
            return type
                .Hierarchy()
                .SelectMany(GetTypeFields)
                .Where(x => x.CloneType != CloneType.Ignore);
        }
    }
}