using System;
using SimplyFast.Reflection.Internal.DelegateBuilders.Expressions;
#if EMIT
using System.Reflection.Emit;
#else
using System.Linq.Expressions;
#endif

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Parameters
{
    internal abstract class ArgLocalVariableParameterMap : ArgParameterMap
    {
        protected readonly bool NeedLocalVariable;
        private readonly Type _methodType;

        protected ArgLocalVariableParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
            SimpleParameterInfo methodParameter)
            : base(delegateParameter, delegateParameterIndex, methodParameter)
        {
            _methodType = methodParameter.Type.RemoveByRef();
            var delegateType = delegateParameter.Type.RemoveByRef();
            if (!delegateType.IsAssignableFrom(_methodType))
                throw new ArgumentException("Invalid type for parameter " + delegateParameterIndex);
            NeedLocalVariable = delegateType != _methodType;
        }

#if EMIT
        
        protected LocalBuilder _localVariable;
        public override void EmitPrepare(ILGenerator generator)
        {
            if (_needLocalVariable)
                _localVariable = generator.DeclareLocal(_methodType);
        }
#else
        protected ParameterExpression LocalVariable;
        public override Expression Prepare(ExpressionBlockBuilder block, ParameterExpression parameter)
        {
            if (!NeedLocalVariable)
                return parameter;
            LocalVariable = Expression.Variable(_methodType);
            block.AddVariable(LocalVariable);
            return LocalVariable;
        }
#endif
    }
}