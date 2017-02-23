using System;
#if EMIT
using System.Reflection.Emit;
using SF.Reflection.Emit;
#endif


namespace SF.Reflection.Internal.DelegateBuilders.Parameters
{
    internal class ArgRefParameterMap : ArgOutParameterMap
    {
        public ArgRefParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
            SimpleParameterInfo methodParameter)
            : base(delegateParameter, delegateParameterIndex, methodParameter)
        {
        }

        protected override void CheckParameters()
        {
            if (!_methodParameter.Type.IsByRef)
                throw new ArgumentException("Invalid methodParameter modifier. Should be Ref.");
            //if (!(_delegateParameter.ParameterType.IsByRef || _del))
            //    throw new ArgumentException(string.Format("Invalid modifier for parameter {0}. Should be Ref or None.",
            //                                              _delegateParameterIndex));
        }

#if EMIT

        public override void EmitFinish(ILGenerator generator)
        {
            if (_delegateParameter.Type.IsByRef)
                base.EmitFinish(generator);
        }

        public override void EmitLoad(ILGenerator generator)
        {
            if (!_delegateParameter.Type.IsByRef && !_needLocalVariable)
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
            var dt = _delegateParameter.Type.RemoveByRef();
            var mt = _methodParameter.Type.RemoveByRef();

            if (_delegateParameter.Type.IsByRef)
                generator.EmitLdind(dt);
            if (dt.IsValueType && !mt.IsValueType)
                generator.EmitBox(dt);
            else if (!mt.IsAssignableFrom(dt))
                generator.EmitUnBoxAnyOrCastClass(mt);
            generator.EmitStloc(_localVariable.LocalIndex);
        }

#endif
    }
}