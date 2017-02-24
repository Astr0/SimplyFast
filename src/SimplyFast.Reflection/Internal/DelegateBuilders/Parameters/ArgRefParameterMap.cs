using System;

#if EMIT
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
#else
using System.Linq.Expressions;
#endif


namespace SimplyFast.Reflection.Internal.DelegateBuilders.Parameters
{
    internal class ArgRefParameterMap : ArgOutParameterMap
    {
        // Method ref, delegate any
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
#else
        public override Expression Prepare(ExpressionBlockBuilder block, ParameterExpression parameter)
        {
            if (_delegateParameter.Type.IsByRef && !_needLocalVariable)
                return parameter;

            var basePrepare = base.Prepare(block, parameter);
            if (!_needLocalVariable)
                return basePrepare;
            var mt = _methodParameter.Type.RemoveByRef();
            var value = _delegateParameter.IsOut
                ? (Expression)Expression.Default(mt)
                : Expression.Convert(parameter, mt);
            var assign = Expression.Assign(basePrepare, value);
            block.Add(assign);
            return basePrepare;
        }

        public override void Finish(ExpressionBlockBuilder block, Expression parameter)
        {
            if (_delegateParameter.Type.IsByRef || _delegateParameter.IsOut)
                base.Finish(block, parameter);
        }
#endif
    }
}