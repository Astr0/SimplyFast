using System;
using System.Reflection;
using System.Reflection.Emit;
using SF.Reflection.Emit;

namespace SF.Reflection.DelegateBuilders
{
    internal class MethodDelegateBuilder : DelegateBuilder
    {
        private readonly MethodInfo _methodInfo;

        public MethodDelegateBuilder(MethodInfo methodInfo, Type delegateType) : base(delegateType)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo");
            _methodInfo = methodInfo;
        }

        #region Overrides of DelegateBuilder

        protected override Delegate CreateExactDelegate()
        {
            return Delegate.CreateDelegate(_delegateType, _methodInfo);
        }

        protected override void EmitInvoke(ILGenerator generator)
        {
            generator.EmitMethodCall(_methodInfo);
        }

        protected override Type GetMethodReturnType()
        {
            return _methodInfo.ReturnType;
        }

        protected override ParameterInfo[] GetMethodParameters()
        {
            return _methodInfo.GetParameters();
        }

        protected override ParameterInfo GetThisParameterForMethod()
        {
            if (_methodInfo.IsStatic)
                return null;
            var declaringType = _methodInfo.DeclaringType;
            // ReSharper disable PossibleNullReferenceException
            return new SimpleParameterInfo(declaringType.IsValueType ? declaringType.MakeByRefType() : declaringType);
            // ReSharper restore PossibleNullReferenceException
        }

        #endregion
    }
}