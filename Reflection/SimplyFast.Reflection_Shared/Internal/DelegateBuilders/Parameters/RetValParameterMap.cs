using System;
using System.Reflection.Emit;
using SF.Reflection.Emit;

namespace SF.Reflection.DelegateBuilders
{
    internal class RetValParameterMap : IDelegateParameterMap
    {
        private readonly Type _delegateReturn;
        private readonly Type _methodReturn;

        public RetValParameterMap(Type delegateReturn, Type methodReturn)
        {
            if (!delegateReturn.IsAssignableFrom(methodReturn))
                throw new Exception("Invalid return type.");

            _delegateReturn = delegateReturn;
            _methodReturn = methodReturn;
        }

        #region Implementation of IDelegateParameterMap

        public void EmitPrepare(ILGenerator generator)
        {
        }

        public void EmitLoad(ILGenerator generator)
        {
        }

        public void EmitFinish(ILGenerator generator)
        {
            if (_methodReturn == _delegateReturn)
                return;
            if (_methodReturn == typeof (void))
                generator.Emit(OpCodes.Ldnull);
            else if (_methodReturn.IsValueType && !_delegateReturn.IsValueType)
                generator.EmitBox(_methodReturn);
        }

        #endregion
    }
}