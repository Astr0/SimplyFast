using System;
using System.Linq;
using System.Reflection;
using SimplyFast.Cloning.Internal.Deep;
using SimplyFast.Reflection;

namespace SimplyFast.Cloning
{
    public static class CloneObjectEx
    {
        public static readonly ICloneObject Ignore = new IgnoreCloneObject();
        public static readonly ICloneObject Copy = new CopyCloneObject();
        public static readonly ICloneObject CopyArray = new CopyArrayCloneObject();

        private static ICloneObject CreateCloneObject(Type genericTypeDefinition, Type arg)
        {
            return genericTypeDefinition.MakeGeneric(arg).CreateInstance<ICloneObject>();
        }

        public static ICloneObject CloneArray(Type elementType)
        {
            return CreateCloneObject(typeof(CloneArrayCloneObject<>), elementType);
        }

        public static CloneType GetCloneType(Type type)
        {
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
                return CloneType.Copy;
            return GetCloneTypeFromAttribute(type) ?? CloneType.Deep;
            // TODO: Nullable?
            //if (entityType.IsNullable()) {
            //    Type underlyingType = entityType.GetNullableUnderlyingType();
            //    return GetPerTypeBehavior(underlyingType);
            //}
        }

        public static CloneType? GetCloneTypeFromAttribute(MemberInfo member)
        {
            var attr = member.GetCustomAttributes<CloneTypeAttribute>().FirstOrDefault();
            return attr?.Type;
        }

        public static ICloneObject DeepCloneObject(Type type)
        {
            return new DeepCloneObject(type);
        }

        private class CopyArrayCloneObject : ICloneObject
        {
            public object Clone(ICloneContext context, object src)
            {
                return ((Array) src).Clone();
            }
        }

        private class CopyCloneObject : ICloneObject
        {
            public object Clone(ICloneContext context, object src)
            {
                return src;
            }
        }

        private class IgnoreCloneObject : ICloneObject
        {
            public object Clone(ICloneContext context, object src)
            {
                return null;
            }
        }

        private class CloneArrayCloneObject<T> : ICloneObject
        {
            public object Clone(ICloneContext context, object src)
            {
                return Array.ConvertAll((T[]) src, x => (T) context.Clone(x));
            }
        }
    }
}