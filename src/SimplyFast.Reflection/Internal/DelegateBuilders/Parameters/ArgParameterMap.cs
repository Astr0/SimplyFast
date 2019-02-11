using System.Diagnostics.CodeAnalysis;
using SimplyFast.Reflection.Internal.DelegateBuilders.Expressions;
#if EMIT
using System.Reflection.Emit;
#else
using System.Linq.Expressions;
#endif

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Parameters
{
    internal abstract class ArgParameterMap
    {
        protected readonly SimpleParameterInfo DelegateParameter;
        protected readonly int DelegateParameterIndex;
        protected readonly SimpleParameterInfo MethodParameter;

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected ArgParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
            SimpleParameterInfo methodParameter)
        {
            DelegateParameter = delegateParameter;
            DelegateParameterIndex = delegateParameterIndex;
            MethodParameter = methodParameter;
            CheckParameters();
        }

        protected abstract void CheckParameters();

        public static ArgParameterMap CreateParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
            SimpleParameterInfo methodParameter)
        {
            if (methodParameter.IsOut)
                return new ArgOutParameterMap(delegateParameter, delegateParameterIndex, methodParameter);
            if (methodParameter.Type.IsByRef)
                return new ArgRefParameterMap(delegateParameter, delegateParameterIndex, methodParameter);
            return new ArgNoneParameterMap(delegateParameter, delegateParameterIndex, methodParameter);
        }

#if EMIT
        public abstract void EmitPrepare(ILGenerator generator);
        public abstract void EmitLoad(ILGenerator generator);
        public abstract void EmitFinish(ILGenerator generator);
#else
        public abstract Expression Prepare(ExpressionBlockBuilder block, ParameterExpression parameter);
        public abstract void Finish(ExpressionBlockBuilder block, Expression parameter);
#endif

    }
}