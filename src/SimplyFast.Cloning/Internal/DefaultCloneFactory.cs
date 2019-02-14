using System;
using System.Collections.Concurrent;

namespace SimplyFast.Cloning.Internal
{
    internal class DefaultCloneFactory
    {
        private readonly ConcurrentDictionary<Type, CloneObject> _cache = new ConcurrentDictionary<Type, CloneObject>();

        public object Clone(ICloneContext context, object src)
        {
            return _cache.GetOrAdd(src.GetType(), CreateObjectClone)(context, src);
        }

        private static CloneObject CreateObjectClone(Type type)
        {
            switch (CloneObjectEx.GetCloneType(type))
            {
                case CloneType.Ignore:
                    return CloneObjectEx.Ignore;
                case CloneType.Copy:
                    return CloneObjectEx.Copy;
                case CloneType.Deep:
                    if (type.IsArray)
                        return CreateArrayClone(type);
                    
                    return CloneObjectEx.DeepCloneObject(type);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static CloneObject CreateArrayClone(Type type)
        {
            var elementType = type.GetElementType();
            switch (CloneObjectEx.GetCloneType(elementType))
            {
                case CloneType.Ignore:
                    return CloneObjectEx.Ignore;
                case CloneType.Copy:
                    return CloneObjectEx.CopyArray;
                case CloneType.Deep:
                    return CloneObjectEx.CloneArray(elementType);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}