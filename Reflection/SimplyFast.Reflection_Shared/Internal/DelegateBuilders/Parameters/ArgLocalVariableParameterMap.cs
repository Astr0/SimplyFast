using System;
#if EMIT
using System.Reflection.Emit;
#endif

namespace SF.Reflection.Internal.DelegateBuilders.Parameters
{
    internal abstract class ArgLocalVariableParameterMap : ArgParameterMap
    {
        private readonly Type _methodType;
        protected readonly bool _needLocalVariable;

        protected ArgLocalVariableParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
            SimpleParameterInfo methodParameter)
            : base(delegateParameter, delegateParameterIndex, methodParameter)
        {
            _methodType = methodParameter.Type.RemoveByRef();
            var delegateType = delegateParameter.Type.RemoveByRef();
            if (!delegateType.IsAssignableFrom(_methodType))
                throw new ArgumentException("Invalid type for parameter " + delegateParameterIndex);
            _needLocalVariable = delegateType != _methodType;
        }

#if EMIT
        protected LocalBuilder _localVariable;
        public override void EmitPrepare(ILGenerator generator)
        {
            if (_needLocalVariable)
                _localVariable = generator.DeclareLocal(_methodType);
        }
#endif
    }
}