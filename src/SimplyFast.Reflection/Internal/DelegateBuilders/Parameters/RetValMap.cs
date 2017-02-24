using System;
#if EMIT
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
#else
using System.Linq.Expressions;
#endif

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Parameters
{
    internal class RetValMap
    {
        private readonly Type _delegateReturn;
        private readonly Type _methodReturn;

        public RetValMap(Type delegateReturn, Type methodReturn)
        {
            if (!delegateReturn.IsAssignableFrom(methodReturn))
                throw new Exception("Invalid return type.");

            _delegateReturn = delegateReturn;
            _methodReturn = methodReturn;
        }

#if EMIT

        public void EmitConvert(ILGenerator generator)
        {
            if (_methodReturn == _delegateReturn)
                return;
            if (_methodReturn == typeof (void))
                generator.Emit(OpCodes.Ldnull);
            else if (_methodReturn.IsValueType && !_delegateReturn.IsValueType)
                generator.EmitBox(_methodReturn);
        }
#else
        private static readonly Expression _void = Expression.Empty();

        public Expression ConvertReturn(Expression retVal)
        {
            if (retVal == null || _methodReturn == typeof(void))
                return _void;
            if (_methodReturn == _delegateReturn)
                return retVal;
            return Expression.Convert(retVal, _delegateReturn);
        }
#endif
    }
}