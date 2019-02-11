using System;
using SimplyFast.Reflection.Internal.DelegateBuilders.Expressions;
#if EMIT
using SimplyFast.Reflection.Emit;
using System.Reflection.Emit;
#else
using System.Linq.Expressions;
#endif

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Parameters
{
    internal class ArgOutParameterMap : ArgLocalVariableParameterMap
    {
        // Method Out, Delegate Out or Ref, Delegate is assignable from Method
        public ArgOutParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
            SimpleParameterInfo methodParameter)
            : base(delegateParameter, delegateParameterIndex, methodParameter)
        {
        }

        protected override void CheckParameters()
        {
            if (!_methodParameter.IsOut)
                throw new ArgumentException("Invalid methodParameter modifier. Should be Out.");
            if (!(_delegateParameter.IsOut || _delegateParameter.Type.IsByRef))
                throw new ArgumentException(
                    $"Invalid modifier for parameter {_delegateParameterIndex}. Should be Out or Ref.");
        }

#if EMIT
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
            var mt = _methodParameter.Type.RemoveByRef();
            var dt = _delegateParameter.Type.RemoveByRef();
            if (mt.IsValueType && !dt.IsValueType)
                generator.EmitBox(mt);
            generator.EmitStind(dt);
        }
#else
        public override void Finish(ExpressionBlockBuilder block, Expression parameter)
        {
            if (!_needLocalVariable)
                return;
            var convertResult = Expression.Convert(_localVariable, _delegateParameter.Type.RemoveByRef());
            var assign = Expression.Assign(parameter, convertResult);
            block.Add(assign);
        }
#endif
    }
}