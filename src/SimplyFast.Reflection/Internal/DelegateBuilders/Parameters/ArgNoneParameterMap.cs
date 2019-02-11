using System;
using SimplyFast.Reflection.Internal.DelegateBuilders.Expressions;
#if EMIT
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
#else
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
            var mt = MethodParameter.Type;
            if (mt.IsByRef)
                throw new ArgumentException("Invalid methodParameter modifier. Should be None.");
            if (DelegateParameter.IsOut)
                throw new ArgumentException($"Invalid modifier for parameter {DelegateParameterIndex}. Should be None or Ref.");

            var dt = DelegateParameter.Type.RemoveByRef();
            if (!dt.IsAssignableFrom(mt) && !mt.IsAssignableFrom(dt))
                throw new ArgumentException("Invalid type for parameter " + DelegateParameterIndex);
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
        public override void Finish(ExpressionBlockBuilder block, Expression parameter)
        {
        }

        public override Expression Prepare(ExpressionBlockBuilder block, ParameterExpression parameter)
        {
            if (MethodParameter.Type == DelegateParameter.Type)
                return parameter;
            return Expression.Convert(parameter, MethodParameter.Type);
        }
#endif
    }
}