using System;
using System.Reflection;
using System.Reflection.Emit;
using SF.Reflection.Emit;

namespace SF.Reflection.DelegateBuilders
{
    internal class ArgNoneParameterMap : ArgParameterMap
    {
        public ArgNoneParameterMap(ParameterInfo delegateParameter, int delegateParameterIndex,
                                   ParameterInfo methodParameter)
            : base(delegateParameter, delegateParameterIndex, methodParameter)
        {
        }

        #region Overrides of ArgParameterMap

        private void EmitLoadToStack(ILGenerator generator)
        {
            generator.EmitLdarg(_delegateParameterIndex);
            if (_delegateParameter.ParameterType.IsByRef)
            {
                generator.EmitLdind(_delegateParameter.ParameterType.RemoveByRef());
            }
        }

        protected override void CheckParameters()
        {
            if (_methodParameter.ParameterType.IsByRef)
                throw new ArgumentException("Invalid methodParameter modifier. Should be None.");
            if (_delegateParameter.IsOut)
                throw new ArgumentException(string.Format("Invalid modifier for parameter {0}. Should be None or Ref.",
                                                          _delegateParameterIndex));

            var dt = _delegateParameter.ParameterType.RemoveByRef();
            var mt = _delegateParameter.ParameterType.RemoveByRef();
            if (!dt.IsAssignableFrom(mt) && !mt.IsAssignableFrom(dt))
                throw new ArgumentException("Invalid type for parameter " + _delegateParameterIndex);
        }

        public override void EmitPrepare(ILGenerator generator)
        {
        }

        public override void EmitLoad(ILGenerator generator)
        {
            EmitLoadToStack(generator);
            var mt = _methodParameter.ParameterType.RemoveByRef();
            var dt = _delegateParameter.ParameterType.RemoveByRef();
            if (dt.IsValueType && !mt.IsValueType)
                generator.EmitBox(dt);
            else if (!mt.IsAssignableFrom(dt))
                generator.EmitUnBoxAnyOrCastClass(mt);
        }

        public override void EmitFinish(ILGenerator generator)
        {
        }

        #endregion
    }
}