using System;
#if EMIT
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
#else
using System.Collections.Generic;
using System.Linq.Expressions;
#endif

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Parameters
{
    internal class ArgNoneParameterMap : ArgParameterMap
    {
        // Method = None, Delegate = Ref or None
        public ArgNoneParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
            SimpleParameterInfo methodParameter)
            : base(delegateParameter, delegateParameterIndex, methodParameter)
        {
        }

        protected override void CheckParameters()
        {
            var mt = _methodParameter.Type;
            if (mt.IsByRef)
                throw new ArgumentException("Invalid methodParameter modifier. Should be None.");
            if (_delegateParameter.IsOut)
                throw new ArgumentException($"Invalid modifier for parameter {_delegateParameterIndex}. Should be None or Ref.");

            var dt = _delegateParameter.Type.RemoveByRef();
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
            var mt = _methodParameter.Type;
            var dt = _delegateParameter.Type.RemoveByRef();
            if (dt.IsValueType && !mt.IsValueType)
                generator.EmitBox(dt);
            else if (!mt.IsAssignableFrom(dt))
                generator.EmitUnBoxAnyOrCastClass(mt);
        }

        public override void EmitFinish(ILGenerator generator)
        {
        }
#else
        public override void Finish(List<Expression> block, Expression parameter)
        {
        }

        public override Expression Prepare(List<Expression> block, ParameterExpression parameter)
        {
            if (_methodParameter.Type == _delegateParameter.Type)
                return parameter;
            return Expression.Convert(parameter, _methodParameter.Type);
        }
#endif
    }
}