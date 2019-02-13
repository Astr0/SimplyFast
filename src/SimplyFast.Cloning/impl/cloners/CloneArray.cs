using System;

namespace Blink.Common.TinyClone
{
    internal class CloneArray<T>: ICloneType<T[]>
    {
        public T[] Clone(CloneContext cloneContext, T[] obj) 
            => Array.ConvertAll(obj, cloneContext.Clone);
    }
}