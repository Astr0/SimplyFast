using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SF.Reflection.DelegateBuilders
{
    internal abstract class ArgParameterMap : IDelegateParameterMap
    {
        protected readonly ParameterInfo _delegateParameter;
        protected readonly int _delegateParameterIndex;
        protected readonly ParameterInfo _methodParameter;

        protected ArgParameterMap(ParameterInfo delegateParameter, int delegateParameterIndex,
                                  ParameterInfo methodParameter)
        {
            _delegateParameter = delegateParameter;
            _delegateParameterIndex = delegateParameterIndex;
            _methodParameter = methodParameter;
            CheckParameters();
        }

        protected abstract void CheckParameters();

        public static ArgParameterMap CreateParameterMap(ParameterInfo delegateParameter, int delegateParameterIndex,
                                                         ParameterInfo methodParameter)
        {
            if (methodParameter.IsOut)
                return new ArgOutParameterMap(delegateParameter, delegateParameterIndex, methodParameter);
            if (methodParameter.ParameterType.IsByRef)
                return new ArgRefParameterMap(delegateParameter, delegateParameterIndex, methodParameter);
            return new ArgNoneParameterMap(delegateParameter, delegateParameterIndex, methodParameter);
        }

        #region Implementation of IDelegateParameterMap

        public abstract void EmitPrepare(ILGenerator generator);

        public abstract void EmitLoad(ILGenerator generator);

        public abstract void EmitFinish(ILGenerator generator);

        #endregion
    }
}