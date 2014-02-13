using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SF.Reflection.DelegateBuilders
{
    internal abstract class ArgLocalVariableParameterMap : ArgParameterMap
    {
        private readonly Type _methodType;
        protected readonly bool _needLocalVariable;
        protected LocalBuilder _localVariable;

        protected ArgLocalVariableParameterMap(ParameterInfo delegateParameter, int delegateParameterIndex,
            ParameterInfo methodParameter)
            : base(delegateParameter, delegateParameterIndex, methodParameter)
        {
            _methodType = methodParameter.ParameterType.RemoveByRef();
            var delegateType = delegateParameter.ParameterType.RemoveByRef();
            if (!delegateType.IsAssignableFrom(_methodType))
                throw new ArgumentException("Invalid type for parameter " + delegateParameterIndex);
            _needLocalVariable = delegateType != _methodType;
        }

        public override void EmitPrepare(ILGenerator generator)
        {
            if (_needLocalVariable)
                _localVariable = generator.DeclareLocal(_methodType);
        }
    }
}