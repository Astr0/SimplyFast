using System;
#if EMIT
using System.Reflection.Emit;
using SF.Reflection.Emit;
#endif

namespace SF.Reflection.Internal.DelegateBuilders.Parameters
{
    internal class ArgNoneParameterMap : ArgParameterMap
    {
        public ArgNoneParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
            SimpleParameterInfo methodParameter)
            : base(delegateParameter, delegateParameterIndex, methodParameter)
        {
        }

        #region Overrides of ArgParameterMap

        protected override void CheckParameters()
        {
            if (_methodParameter.Type.IsByRef)
                throw new ArgumentException("Invalid methodParameter modifier. Should be None.");
            if (_delegateParameter.IsOut)
                throw new ArgumentException($"Invalid modifier for parameter {_delegateParameterIndex}. Should be None or Ref.");

            var dt = _delegateParameter.Type.RemoveByRef();
            var mt = _delegateParameter.Type.RemoveByRef();
            if (!dt.IsAssignableFrom(mt) && !mt.IsAssignableFrom(dt))
                throw new ArgumentException("Invalid type for parameter " + _delegateParameterIndex);
        }

#if EMIT
        private void EmitLoadToStack(ILGenerator generator)
        {
            generator.EmitLdarg(_delegateParameterIndex);
            if (_delegateParameter.Type.IsByRef)
            {
                generator.EmitLdind(_delegateParameter.Type.RemoveByRef());
            }
        }

        public override void EmitPrepare(ILGenerator generator)
        {
        }

        public override void EmitLoad(ILGenerator generator)
        {
            EmitLoadToStack(generator);
            var mt = _methodParameter.Type.RemoveByRef();
            var dt = _delegateParameter.Type.RemoveByRef();
            if (dt.IsValueType && !mt.IsValueType)
                generator.EmitBox(dt);
            else if (!mt.IsAssignableFrom(dt))
                generator.EmitUnBoxAnyOrCastClass(mt);
        }

        public override void EmitFinish(ILGenerator generator)
        {
        }
#endif

        #endregion
    }
}