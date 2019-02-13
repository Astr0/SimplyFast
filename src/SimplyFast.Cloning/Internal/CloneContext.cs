using System;
using System.Collections.Generic;

namespace SimplyFast.Cloning.Internal
{
    internal class CloneContext: ICloneContext
    {
        private readonly ICloneObject _cloneObject;
        private readonly object _cloning = new object();

        private readonly Dictionary<object, object> _objects = new Dictionary<object, object>();

        public CloneContext(ICloneObject cloneObject)
        {
            _cloneObject = cloneObject;
        }

        public object Clone(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return default;
            }
            if (_objects.TryGetValue(obj, out var clonedObj)) {
                if (ReferenceEquals(clonedObj, _cloning))
                    throw new InvalidOperationException("Circular reference found");
                return clonedObj;
            }

            _objects.Add(obj, _cloning);
            
            // clone
            clonedObj = _cloneObject.Clone(this, obj);

            _objects[obj] = clonedObj;

            return clonedObj;
        }
    }
}