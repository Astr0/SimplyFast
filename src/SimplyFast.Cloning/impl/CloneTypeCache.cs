using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Blink.Common.Fast.Reflection;

namespace Blink.Common.TinyClone
{
    internal class CloneTypeCache
    {
        private readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();

        public ICloneType<T> Get<T>()
        {
            var result = _cache.GetOrAdd(typeof(T), CreateCloneType);
            return (ICloneType<T>) result;
        }

        public static CloneType GetCloneType(Type type)
        {
            if (type.IsPrimitive || type.IsEnum || type == typeof (string))
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
            var attr = member.GetCustomAttributes<CloneOverrideAttribute>().FirstOrDefault();
            return attr?.Type;
        }

        private static object ActivateCloneType(Type generic, Type arg)
        {
            return generic
                .Fast()
                .MakeGeneric(arg)
                .Constructors.Default().Invoke();
        }

        private static object CreateCloneType(Type type)
        {
            switch (GetCloneType(type))
            {
                case CloneType.Ignore:
                    return ActivateCloneType(typeof(CloneIgnore<>), type);
                case CloneType.Copy:
                    return ActivateCloneType(typeof(CloneCopy<>), type);
                case CloneType.Deep:
                    if (type.IsArray)
                        return CreateArrayClone(type);
                    // TODO: Nullable
                    return ActivateCloneType(typeof(CloneDeep<>), type);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static object CreateArrayClone(Type type)
        {
            var elementType = type.GetElementType();
            switch (GetCloneType(elementType))
            {
                case CloneType.Ignore:
                    return ActivateCloneType(typeof(CloneIgnore<>), type);
                case CloneType.Copy:
                    return ActivateCloneType(typeof(CloneCopy<>), type);
                case CloneType.Deep:
                    return ActivateCloneType(typeof(CloneArray<>), elementType);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}