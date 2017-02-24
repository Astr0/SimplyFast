using System;
using System.Reflection;
using SimplyFast.Reflection.Internal.DelegateBuilders.Parameters;

#if EMIT
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
#endif

namespace SimplyFast.Reflection.Internal.DelegateBuilders
{
    internal class MethodDelegateBuilder : DelegateBuilder
    {
        private readonly MethodInfo _methodInfo;

        public MethodDelegateBuilder(MethodInfo methodInfo, Type delegateType) : base(delegateType)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
            _methodInfo = methodInfo;
        }

        #region Overrides of DelegateBuilder

#if EMIT
        protected override Delegate CreateExactDelegate()
        {
            var declaring = _methodInfo.DeclaringType;
            // avoid stupid issue in .Net framework...
            if (declaring != null && declaring.IsInterface && _methodInfo.IsGenericMethod)
                return CreateCastDelegate();
            return Delegate.CreateDelegate(_delegateType, _methodInfo);
        }

        protected override void EmitInvoke(ILGenerator generator)
        {
            generator.EmitMethodCall(_methodInfo);
        }
#endif

        protected override Type GetMethodReturnType()
        {
            return _methodInfo.ReturnType;
        }

        protected override SimpleParameterInfo[] GetMethodParameters()
        {
            return SimpleParameterInfo.FromParameters(_methodInfo.GetParameters());
        }

        protected override Type GetThisParameterForMethod()
        {
            if (_methodInfo.IsStatic)
                return null;
            var declaringType = _methodInfo.DeclaringType;
            // ReSharper disable PossibleNullReferenceException
            return declaringType.IsValueType() ? declaringType.MakeByRefType() : declaringType;
            // ReSharper restore PossibleNullReferenceException
        }

#endregion
    }
}