using System;
using System.Collections.Generic;

namespace Blink.Common.TinyClone
{
    internal class CloneContext
    {
        private readonly CloneTypeCache _cloneTypeCache;
        private readonly object _cloning = new object();

        private readonly Dictionary<object, object> _objects = new Dictionary<object, object>();

        public CloneContext(CloneTypeCache cloneTypeCache)
        {
            _cloneTypeCache = cloneTypeCache;
        }

        public T Clone<T>(T obj)
        {
            if (typeof(T).IsValueType)
                return obj;
            if (ReferenceEquals(obj, null))
            {
                return default;
            }
            if (_objects.TryGetValue(obj, out var resultObj)) {
                if (ReferenceEquals(resultObj, _cloning))
                    throw new InvalidOperationException("Circular reference found");
                return (T)resultObj;
            }

            _objects.Add(obj, _cloning);
            
            // clone
            var result = DoClone(obj);

            _objects[obj] = result;

            return result;
        }

        private T DoClone<T>(T obj)
        {
            return _cloneTypeCache.Get<T>().Clone(this, obj);
        }
    }
}