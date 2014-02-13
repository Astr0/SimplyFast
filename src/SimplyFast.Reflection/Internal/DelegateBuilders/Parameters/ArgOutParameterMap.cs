using System;
using System.Reflection;
using System.Reflection.Emit;
using SF.Reflection.Emit;

namespace SF.Reflection.DelegateBuilders
{
    internal class ArgOutParameterMap : ArgLocalVariableParameterMap
    {
        public ArgOutParameterMap(ParameterInfo delegateParameter, int delegateParameterIndex,
            ParameterInfo methodParameter)
            : base(delegateParameter, delegateParameterIndex, methodParameter)
        {
        }

        #region Overrides of ArgParameterMap

        protected override void CheckParameters()
        {
            if (!_methodParameter.IsOut)
                throw new ArgumentException("Invalid methodParameter modifier. Should be Out.");
            if (!(_delegateParameter.IsOut || _delegateParameter.ParameterType.IsByRef))
                throw new ArgumentException(string.Format("Invalid modifier for parameter {0}. Should be Out or Ref.",
                    _delegateParameterIndex));
        }

        public override void EmitLoad(ILGenerator generator)
        {
            if (_needLocalVariable)
            {
                generator.EmitLdloca(_localVariable.LocalIndex);
            }
            else
            {
                generator.EmitLdarg(_delegateParameterIndex);
            }
        }

        public override void EmitFinish(ILGenerator generator)
        {
            if (!_needLocalVariable)
                return;
            generator.EmitLdarg(_delegateParameterIndex);
            generator.EmitLdloc(_localVariable.LocalIndex);
            var mt = _methodParameter.ParameterType.RemoveByRef();
            var dt = _delegateParameter.ParameterType.RemoveByRef();
            if (mt.IsValueType && !dt.IsValueType)
                generator.EmitBox(mt);
            generator.EmitStind(dt);
        }

        #endregion
    }
}