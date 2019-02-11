using System;

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Parameters
{
    internal struct RetValMap
    {
        public readonly Type DelegateReturn;
        public readonly Type MethodReturn;

        public RetValMap(Type delegateReturn, Type methodReturn)
        {
            if (delegateReturn != typeof(void) && !delegateReturn.IsAssignableFrom(methodReturn))
                throw new Exception("Invalid return type.");

            DelegateReturn = delegateReturn;
            MethodReturn = methodReturn;
        }

#if EMIT

        public void EmitConvert(ILGenerator generator)
        {
            if (_methodReturn == _delegateReturn || _delegateReturn == typeof(void))
                return;
            if (_methodReturn == typeof (void))
                generator.Emit(OpCodes.Ldnull);
            else if (_methodReturn.IsValueType && !_delegateReturn.IsValueType)
                generator.EmitBox(_methodReturn);
        }
#endif
    }
}