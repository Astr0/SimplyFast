using System;

namespace SimplyFast.Cloning.Internal.Deep
{
    public class DeepCloneEmit
    {
        private readonly Func<ICloneContext, object, object> _clone;

        public DeepCloneEmit(Type type)
        {
        }

        public object Clone(ICloneContext context, object src)
        {
            return _clone(context, src);
        }
    }
}