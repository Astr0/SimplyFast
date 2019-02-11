using System;

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Maps
{
    internal struct RetValMap
    {
        public readonly Type Delegate;
        public readonly Type Method;

        public RetValMap(Type @delegate, Type method)
        {
            if (@delegate != typeof(void) && !@delegate.IsAssignableFrom(method))
                throw new Exception("Invalid return type.");

            Delegate = @delegate;
            Method = method;
        }

        public bool Matches => Delegate == Method;
    }
}