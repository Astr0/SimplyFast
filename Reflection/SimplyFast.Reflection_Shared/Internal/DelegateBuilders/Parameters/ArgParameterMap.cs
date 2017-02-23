using System.Diagnostics.CodeAnalysis;
#if EMIT
using System.Reflection.Emit;
#endif

namespace SF.Reflection.Internal.DelegateBuilders.Parameters
{
    internal abstract class ArgParameterMap : IDelegateParameterMap
    {
        protected readonly SimpleParameterInfo _delegateParameter;
        protected readonly int _delegateParameterIndex;
        protected readonly SimpleParameterInfo _methodParameter;

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected ArgParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
            SimpleParameterInfo methodParameter)
        {
            _delegateParameter = delegateParameter;
            _delegateParameterIndex = delegateParameterIndex;
            _methodParameter = methodParameter;
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

#endif
        
    }
}