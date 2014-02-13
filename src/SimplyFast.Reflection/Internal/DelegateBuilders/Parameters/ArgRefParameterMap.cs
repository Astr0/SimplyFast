using System;
using System.Reflection;
using System.Reflection.Emit;
using SF.Reflection.Emit;

namespace SF.Reflection.DelegateBuilders
{
    internal class ArgRefParameterMap : ArgOutParameterMap
    {
        public ArgRefParameterMap(ParameterInfo delegateParameter, int delegateParameterIndex,
            ParameterInfo methodParameter)
            : base(delegateParameter, delegateParameterIndex, methodParameter)
        {
        }

        protected override void CheckParameters()
        {
            if (!_methodParameter.ParameterType.IsByRef)
                throw new ArgumentException("Invalid methodParameter modifier. Should be Ref.");
            //if (!(_delegateParameter.ParameterType.IsByRef || _del))
            //    throw new ArgumentException(string.Format("Invalid modifier for parameter {0}. Should be Ref or None.",
            //                                              _delegateParameterIndex));
        }

        #region Overrides of ArgParameterMap

        public override void EmitFinish(ILGenerator generator)
        {
            if (_delegateParameter.ParameterType.IsByRef)
                base.EmitFinish(generator);
        }

        public override void EmitLoad(ILGenerator generator)
        {
            if (!_delegateParameter.ParameterType.IsByRef && !_needLocalVariable)
                generator.EmitLdarga(_delegateParameterIndex);
            else
                base.EmitLoad(generator);
        }

        public override void EmitPrepare(ILGenerator generator)
        {
            base.EmitPrepare(generator);
            if (!_needLocalVariable)
                return;
            generator.EmitLdarg(_delegateParameterIndex);
            var dt = _delegateParameter.ParameterType.RemoveByRef();
            var mt = _methodParameter.ParameterType.RemoveByRef();

            if (_delegateParameter.ParameterType.IsByRef)
                generator.EmitLdind(dt);
            if (dt.IsValueType && !mt.IsValueType)
                generator.EmitBox(dt);
            else if (!mt.IsAssignableFrom(dt))
                generator.EmitUnBoxAnyOrCastClass(mt);
            generator.EmitStloc(_localVariable.LocalIndex);
        }

        #endregion
    }
}